using Differenti8.DataLayer.Profile;
using Differenti8.Presenters;

namespace Differenti8.Views
{
    internal interface ICompareViewer
    {
		bool PreviousSelections_IndexChanged_Event_On { set; }
		object PreviousSelections_DataSource { set; }
		string PreviousSelections_DisplayMember { set; }
		string PreviousSelections_SelectedString { set; }
		UserSetting PreviousSelections_SelectedItem { get; }
		string PreviousSelections_Text { get; set; }
		int PreviousSelections_Count { get; }
		string NewBaseDir { get; set; }
		string OldBaseDir { get; set; }
		string NewFile { get; set; }
		string OldFile { get; set; }
		long MinChars { get; set; }
		long MinLines { get; set; }
		long LimitCharacters { get; set; }
        long LimitLines { get; set; }
        long SubLineMatchLimit { get; set; }
        bool CompleteLines { get; set; }
		Presenter.CursorModes CursorMode { get; set; }
		ProgressLink CurrentActivityProgress{ get; set; }
		ProgressLink OverallProgress { get; set; }
		string Caption { get; set; }
		string StatusMessage { get; set; }
		event Presenter.PreviousSelectionSelectedIndexChangedHandler OnPreviousSelectionSelectedIndexChanged;
		event Presenter.RemovePreviousSelectionHandler OnRemovePreviousSelection;
		event Presenter.ChooseNewBaseDirectoryHandler OnChooseNewBaseDirectory;
		event Presenter.ChooseOldBaseDirectoryHandler OnChooseOldBaseDirectory;
        event Presenter.CopyNewBaseDirectoryHandler OnCopyNewBaseDirectory;
        event Presenter.CopyOldBaseDirectoryHandler OnCopyOldBaseDirectory;
        event Presenter.ChooseNewFileHandler OnChooseNewFile;
		event Presenter.ChooseOldFileHandler OnChooseOldFile;
		event Presenter.CompareActionHandler OnCompareAction;
        event Presenter.CompareMeldActionHandler OnCompareMeldAction;
        event Presenter.CancelActionHandler OnCancelAction;
        void ClearErrors();
		void SetFieldError(string fieldName, string message);
		string BrowseFolder(string initialDirectory, string defaultFolder);
        string BrowseFile(string initialDirectory, string filter, string defaultFileSpec);
        Presenter.MsgResult DisplayMessageBox(string message, string caption, Presenter.MsgButtons buttons, Presenter.MsgIcon icon);
        void EnableControls(bool enabled);
    }
}