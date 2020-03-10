using Git8.DataLayer.Profile;
using Git8.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Git8.Views
{
    public partial class frmRepository : Form
    {
        public frmRepository()
        {
            InitializeComponent();
            InitialiseFields();
        }

        #region Event Handlers.
        private void cboRepositoryName_SelectedIndexChanged(object sender, EventArgs e)
        {
            Administrator.ProfileManager.RepositorySettings.Select(cboRepositoryName.Text);
            Administrator.ProfileManager.Reload();                
            InitialiseFields();
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            SaveFields();
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (Administrator.ProfileManager.RepositorySettings.SelectedItem != null)
            {
                Administrator.ProfileManager.RepositorySettings.Delete(Administrator.ProfileManager.RepositorySettings.SelectedItem.Key);
            }
            InitialiseFields();
        }
        #endregion

        #region Private Methods.
        private void InitialiseFields()
        {
            InitialiseRepositoryNameField();
            InitialiseRemoteUrlField();
            InitialiseLocalPathField();
        }

        private void InitialiseRepositoryNameField()
        {
            this.cboRepositoryName.SelectedIndexChanged -= new System.EventHandler(this.cboRepositoryName_SelectedIndexChanged);
            if(Administrator.ProfileManager.RepositorySettings.Keys.Count() > 0 && Administrator.ProfileManager.RepositorySettings.SelectedItem != null)
            {
                List<string> sortedRepositorySettings = Administrator.ProfileManager.RepositorySettings.Keys.ToList();
                sortedRepositorySettings.Sort();
                //cboRepositoryName.DataSource = Administrator.ProfileManager.RepositorySettings.Keys.ToList();
                cboRepositoryName.DataSource = sortedRepositorySettings;
                cboRepositoryName.DropDownStyle = ComboBoxStyle.DropDown;
                setCboRepositoryNameText(Administrator.ProfileManager.RepositorySettings.SelectedKey);
            }
            else
            {
                setCboRepositoryNameText(string.Empty);
            }
        }

        private void InitialiseRemoteUrlField()
        {
            if (Administrator.ProfileManager.RepositorySettings.SelectedItem != null)
            {
                setTxtRemoteUrlText(Administrator.ProfileManager.RepositorySettings.SelectedItem.RemoteUrl);
            }
            else
            {
                setTxtRemoteUrlText(string.Empty);
            }
        }

        private void InitialiseLocalPathField()
        {
            if (Administrator.ProfileManager.RepositorySettings.SelectedItem != null)
            {
                setTxtLocalPathText(Administrator.ProfileManager.RepositorySettings.SelectedItem.LocalPath);
            }
            else
            {
                setTxtLocalPathText(string.Empty);
            }
        }

        private void setCboRepositoryNameText(string text)
        {
            this.cboRepositoryName.SelectedIndexChanged -= new System.EventHandler(this.cboRepositoryName_SelectedIndexChanged);
            cboRepositoryName.Text = text;
            this.cboRepositoryName.SelectedIndexChanged += new System.EventHandler(this.cboRepositoryName_SelectedIndexChanged);
        }
        private void setTxtRemoteUrlText(string text)
        {
            txtRemoteUrl.Text = text;
        }
        private void setTxtLocalPathText(string text)
        {
            txtLocalPath.Text = text;
        }

        private void SaveFields()
        {
            string repositoryName = cboRepositoryName.Text;
            string remoteUrl = txtRemoteUrl.Text;
            string localPath = txtLocalPath.Text;
            if (!Administrator.ProfileManager.CommandSettings.Keys.Contains(repositoryName))
            {
                RepositorySetting repositorySetting = new RepositorySetting
                {
                    Key = repositoryName,
                    Name = repositoryName,
                    RemoteUrl = remoteUrl,
                    LocalPath = localPath
                };
                Administrator.ProfileManager.RepositorySettings.Persist(repositorySetting);
                InitialiseFields();
                Administrator.ProfileManager.Reload();
            }
            else
            {
                Administrator.ProfileManager.RepositorySettings.SelectedItem.Name = repositoryName;
                Administrator.ProfileManager.RepositorySettings.SelectedItem.RemoteUrl = remoteUrl;
                Administrator.ProfileManager.RepositorySettings.SelectedItem.LocalPath = localPath;
                Administrator.ProfileManager.RepositorySettings.Save();
            }
        }
        #endregion
    }
}