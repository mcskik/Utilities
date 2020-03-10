using Copy8.DataLayer.Profile;
using Copy8.Presenters;
using System.Collections.Specialized;

namespace Copy8.Views
{
    internal interface ICopyViewer
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
		UserSetting.CopyRuleEnum CopyRule { get; set; }
        bool MonitoredTypesOnly { get; set; }
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
		event Presenter.CopyActionHandler OnCopyAction;
        event Presenter.DeleteActionHandler OnDeleteAction;
        event Presenter.SynchronizePreventDeleteActionHandler OnSynchronizePreventDeleteAction;
        event Presenter.SynchronizeActionHandler OnSynchronizeAction;
        event Presenter.SynchronizeAllActionHandler OnSynchronizeAllAction;
        event Presenter.SynchronizePlusActionHandler OnSynchronizePlusAction;
        event Presenter.SynchronizePlusAllActionHandler OnSynchronizePlusAllAction;
        event Presenter.CancelActionHandler OnCancelAction;
        void ClearErrors();
		void SetFieldError(string fieldName, string message);
		string BrowseFolder(string initialDirectory, string defaultFolder);
        string BrowseFile(string initialDirectory, string filter, string defaultFileSpec);
        Presenter.MsgResult DisplayMessageBox(string message, string caption, Presenter.MsgButtons buttons, Presenter.MsgIcon icon);
        void EnableControls(bool enabled);
    }
}