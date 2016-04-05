using Microsoft.Win32;
using ScriptExecutor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CdsModelDeployer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _configFile;
        DeploymentConfig _config;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonLoadConfig_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select config file";
            dialog.Filter = "Xml files (*.xml)|*.xml";
            dialog.FileName = _configFile;
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value == true)
            {
                _configFile = dialog.FileName;

                string xml = File.ReadAllText(_configFile);
                var s = new GenericSerializer<DeploymentConfig>();
                _config = s.Deserialize(xml);
                DisplayConfigSettings();

            }
}
        private void DisplayConfigSettings()
        {
            List<ConfigItem> configItems = new List<ConfigItem>();
            configItems.Add(new ConfigItem() { Name = "Script Folder:", Value = _config.ScriptFolder });
            configItems.Add(new ConfigItem() { Name = "Target SQL Server:", Value = _config.TargetSqlServerName });
            configItems.Add(new ConfigItem() { Name = "CDS Model DB Name:", Value = _config.CdsModelDbName });
            configItems.Add(new ConfigItem() { Name = "IF Datamart DB Name:", Value = _config.IfDataMartDbName });
            foreach (var replacement in _config.Replacements)
            {
                configItems.Add(new ConfigItem()
                {
                    Name = "Replacement:",
                    Value = string.Format("{0}=>{1}", replacement.SearchTerm, replacement.ReplacementTerm)
                });
            }
            listBoxConfig.ItemsSource = configItems;
        }

        private void buttonValidate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sb = new StringBuilder();
                string dbCheckDBSql = "select DB_ID('{0}')";
                //Validate server, credentials
                var sqlExec = new SqlExecutor(this.ConnectionString);

                //Validate CDS Model database

                if (sqlExec.ExecuteScalar(string.Format(dbCheckDBSql, _config.CdsModelDbName)) == null)
                {
                    sb.AppendLine("CDS Model Database: " + _config.CdsModelDbName + " is not valid.");
                }

                //Validate IF Datamart Database 
                if (sqlExec.ExecuteScalar(string.Format(dbCheckDBSql, _config.IfDataMartDbName)) == null)
                {
                    sb.AppendLine("IF Datamart Database: " + _config.IfDataMartDbName + " is not valid.");
                }

                //Validate Script Folder
                if (!Directory.Exists(_config.ScriptFolder))
                {
                    sb.AppendLine("Script Folder: " + _config.IfDataMartDbName + " is not valid.");
                }
                //validate database replacements
                foreach (var replacement in _config.Replacements)
                {
                    //Could use linq but let's keep things simple
                    if (replacement.IsDatabaseName)
                    {
                        if (sqlExec.ExecuteScalar(string.Format(dbCheckDBSql, replacement.ReplacementTerm)) == null)
                        {
                            sb.AppendLine("Replacement Database: " + replacement.ReplacementTerm + " is not valid.");
                        }
                    }
                }
                
                if (sb.ToString() == string.Empty)
                {
                    //All is well
                    sb.AppendLine("Deployment configuration settings are valid.");
                }
                textBlockValidation.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }
        }

        private string ConnectionString
        {
            get
            {
                string format = "Server={0};Database={1};User Id={2};Password = {3}; ";
                return string.Format(format, _config.TargetSqlServerName, _config.CdsModelDbName,
                    textBoxUserName.Text, passwordBox.Password);
            }
        }
    }
}
