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
            configItems.Add(new ConfigItem() { Name = "Script Archive Folder:", Value = _config.ScriptFolder });
            configItems.Add(new ConfigItem() { Name = "Target SQL Server:", Value = _config.TargetSqlServerName });
            configItems.Add(new ConfigItem() { Name = "CDS Model DB Name:", Value = _config.CdsModelDbName });
            foreach (var replacement in _config.Replacements)
            {
                string db = string.Empty;
                if (replacement.IsDatabaseName)
                {
                    db = " [DB]";
                }
                configItems.Add(new ConfigItem()
                {
                    Name = "Replacement" + db +":",
                    Value = string.Format("{0}=>{1}", replacement.SearchTerm, replacement.ReplacementTerm)
                });
            }
            listBoxConfig.ItemsSource = configItems;
        }

        private void buttonValidate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isValid = true;
                var sb = new StringBuilder();
                string dbCheckDBSql = "select DB_ID('{0}')";
                //Validate server, credentials
                var sqlExec = new SqlExecutor(this.ConnectionString);

                //Validate CDS Model database

                if (sqlExec.ExecuteScalar(string.Format(dbCheckDBSql, _config.CdsModelDbName)) == string.Empty)
                {
                    isValid = false;
                    sb.AppendLine("CDS Model Database: " + _config.CdsModelDbName + " is not valid.");
                }

                //Validate Script Folder
                if (!Directory.Exists(_config.ScriptFolder))
                {
                    isValid = false;
                    sb.AppendLine("Script Folder: " + _config.ScriptFolder + " is not valid.");
                }
                else
                {
                    int count = this.GetSqlFiles().Count;
                    sb.AppendLine("Script Folder has " + count.ToString() + " file(s).");
                }
                //Validate Script Archive Folder
                if (!Directory.Exists(_config.ScriptArchiveFolder))
                {
                    isValid = false;
                    sb.AppendLine("Script Archive Folder: " + _config.ScriptArchiveFolder + " is not valid.");
                }

                //validate database replacements
                foreach (var replacement in _config.Replacements)
                {
                    //Could use linq but let's keep things simple
                    if (replacement.IsDatabaseName)
                    {
                        if (sqlExec.ExecuteScalar(string.Format(dbCheckDBSql, replacement.ReplacementTerm)) == string.Empty)
                        {
                            isValid = false;
                            sb.AppendLine("Replacement Database: " + replacement.ReplacementTerm + " is not valid.");
                        }
                    }
                }
                
                if (isValid)
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

        private List<string> GetSqlFiles()
        {
           return  Directory.GetFiles(_config.ScriptFolder, "*.sql").ToList();
        }

        private string ConnectionString
        {
            get
            {
                if (checkBoxSqlServerSecurity.IsChecked.HasValue && checkBoxSqlServerSecurity.IsChecked.Value)
                {
                    string formatSqlSecurity = "Server={0};Database={1};User Id={2};Password = {3}; ";
                    return string.Format(formatSqlSecurity, _config.TargetSqlServerName, _config.CdsModelDbName,
                        textBoxUserName.Text, passwordBox.Password);
                }
                else
                {
                    string formatIntegratedSecurity = "Server={0};Database={1};Trusted_Connection=True;";
                    return string.Format(formatIntegratedSecurity, _config.TargetSqlServerName, _config.CdsModelDbName);
                }
            }
        }

        private void buttonExecute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Clear the rich text box
                TextRange txt = new TextRange(richTextBoxResults.Document.ContentStart, richTextBoxResults.Document.ContentEnd);
                txt.Text = "";
                //Setup the deployer
                var sqlExec = new SqlExecutor(this.ConnectionString);
                var fileUtility = new FileUtility();
                var deployer = new Deployer(sqlExec, fileUtility);
                //Execute the scripts, a stringbuilder is returned that contains the log
                var sb = deployer.ExecuteFileList(this.GetSqlFiles(), _config.Replacements, 
                    _config.ScriptArchiveFolder, checkBoxStopOnException.IsChecked.Value);
                txt.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                richTextBoxResults.AppendText(ex.Message);
            }
        }

        private void CreateArchiveFolder()
        {
            Directory.CreateDirectory(_config.ScriptFolder + @"\ExecutedScripts");
        }
    }
}
