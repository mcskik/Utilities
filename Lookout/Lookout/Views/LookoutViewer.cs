using Lookout.DataLayer.Profile;
using Lookout.Presenters;
using System.Collections.Specialized;

namespace Lookout.Views
{
    internal interface ILookoutViewer
    {
		bool PreviousSelections_IndexChanged_Event_On { set; }
		object PreviousSelections_DataSource { set; }
		string PreviousSelections_DisplayMember { set; }
		string PreviousSelections_SelectedString { set; }
		UserSetting PreviousSelections_SelectedItem { get; }
		string PreviousSelections_Text { get; set; }
		int PreviousSelections_Count { get; }
		string TargetOutlookFolder { get; set; }
        string CriteriaName { get; set; }
        string SenderCriteria { get; set; }
        string SubjectCriteria { get; set; }
        string BodyCriteria { get; set; }
        bool UseMapping { get; set; }
        Presenter.CursorModes CursorMode { get; set; }
		ProgressLink CurrentActivityProgress{ get; set; }
		ProgressLink OverallProgress { get; set; }
		string Caption { get; set; }
		string StatusMessage { get; set; }
		event Presenter.PreviousSelectionSelectedIndexChangedHandler OnPreviousSelectionSelectedIndexChanged;
		event Presenter.RemovePreviousSelectionHandler OnRemovePreviousSelection;
		event Presenter.ChooseSourceFolderHandler OnChooseSourceFolder;
		event Presenter.ChooseTargetFolderHandler OnChooseApkFolder;
        event Presenter.CopySourceFolderHandler OnCopySourceFolder;
        event Presenter.CopyTargetFolderHandler OnCopyTargetFolder;
        event Presenter.ChooseReflateFolderHandler OnChooseReflateFolder;
        event Presenter.ChooseTargetFileHandler OnChooseApkFile;
        event Presenter.CopyActionHandler OnCompressAction;
        event Presenter.MoveActionHandler OnMoveAction;
        event Presenter.CancelActionHandler OnCancelAction;
        void ClearErrors();
		void SetFieldError(string fieldName, string message);
		string BrowseFolder(string initialDirectory, string defaultFolder);
        string BrowseFile(string initialDirectory, string filter, string defaultFileSpec);
        Presenter.MsgResult DisplayMessageBox(string message, string caption, Presenter.MsgButtons buttons, Presenter.MsgIcon icon);
        void EnableControls(bool enabled);
    }
}