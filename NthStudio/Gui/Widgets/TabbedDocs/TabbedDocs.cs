using System;
using System.IO;
using System.Collections.Generic;
using NthStudio.Gui.Widgets.TextEditor;
using NthStudio.Gui.Widgets.TabStrip;
using NthDimension.Forms;
using NthStudio.Gui.Widgets.TextEditor.Document.Selection;
using NthStudio.Gui.Widgets.TextEditor.Document.FoldingStrategy;

namespace NthStudio.Gui.Widgets
{
    public delegate void CursorPositionChangedHandler(int lin, int col, int car);

    public class TabbedDocs : NthStudio.Gui.Widgets.TabStrip.TabStrip
    {
        public event CursorPositionChangedHandler CursorPositionChangedEvent;

        /// <summary>This variable holds the settings (whether to show line numbers,
        /// etc.) that all editor controls share.</summary>
        ITextEditorProperties _editorSettings;

        public TabbedDocs()
        {
        }

        /// <summary>Returns the currently displayed editor, or null if none are open</summary>
        public NthStudio.Gui.Widgets.TextEditor.TextEditor ActiveEditor
        {
            get
            {
                /*if (fileTabs.TabPages.Count == 0) return null;
				return fileTabs.SelectedTab.Controls.OfType<TextEditorControl>().FirstOrDefault();*/

                if (Items.Count == 0 || SelectedItem == null)
                    return null;
                foreach (NthStudio.Gui.Widgets.TextEditor.TextEditor tec in SelectedItem.Widgets)
                    return tec;
                return null;
            }
        }

        private NthStudio.Gui.Widgets.TextEditor.TextEditor AddNewTextEditor(string fileName)
        {
            string title = Path.GetFileName(fileName);
            var tabItem = new TabStripItem();
            tabItem.Title = title;
            tabItem.Tag = fileName;
            tabItem.TabPageToolTipText = fileName;
            tabItem.BGColor = BGColor;
            tabItem.FGColor = FGColor;
            tabItem.PaintBackGround = PaintBackGround;
            var editor = new NthStudio.Gui.Widgets.TextEditor.TextEditor();
            if (Environment.OSVersion.Platform != PlatformID.Unix)
                editor.Font = new NanoFont(NanoFont.DefaultRegular, 11f);

            editor.Dock = EDocking.Fill;
            editor.IsReadOnly = false;
            editor.Document.DocumentChanged +=
                new DocumentEventHandler((sender, e) =>
                {
                    SetModifiedFlag(editor, true);
                    if (editor != null && editor.Document.FoldingManager.FoldingStrategy != null)
                        editor.Document.FoldingManager.UpdateFoldings(null, null);

                });
            editor.ActiveTextArea.TextCanvas.SelectionManager.SelectionChanged +=
                      editor_ActiveTextAreaControl_TextArea_SelectionManager_SelectionChanged;

            tabItem.Widgets.Add(editor);
            AddTab(tabItem);

            if (_editorSettings == null)
            {
                _editorSettings = editor.TextEditorProperties;
                OnSettingsChanged();
            }
            else
                editor.TextEditorProperties = _editorSettings;

            editor.ActiveTextArea.Caret.PositionChanged += delegate (object sender, EventArgs e)
            {
                if (CursorPositionChangedEvent != null)
                {
                    TextArea activeTextAreaControl = editor.ActiveTextArea;
                    int line = activeTextAreaControl.Caret.Line;
                    int col = activeTextAreaControl.Caret.Column;
                    int car = activeTextAreaControl.TextCanvas.TextView.GetVisualColumn(line, col);
                    CursorPositionChangedEvent(line, col, car);
                }
            };

            this.SelectedItem = tabItem;
            editor.Focus();
            // When a tab page gets the focus, move the focus to the editor control
            // instead when it gets the Enter (focus) event. I use BeginInvoke
            // because changing the focus directly in the Enter handler doesn't
            // work.
            /*tabItem.Enter +=
				new EventHandler((sender, e) => {
									var page = ((Panel)sender);
									page.BeginInvoke(new Action<Panel>(p => p.Controls[0].Focus()), page);
								 });
			*/
            //editor.Text = "Esto es una prueba";
            return editor;
        }

        /// <summary>Show current settings on the Options menu</summary>
        /// <remarks>We don't have to sync settings between the editors because
        /// they all share the same DefaultTextEditorProperties object.</remarks>
        private void OnSettingsChanged()
        {
            /*if (MenuOptionsShowSpacesTabs != null)
				MenuOptionsShowSpacesTabs.Checked = _editorSettings.ShowSpaces;
			if (MenuOptionsShowLineNumbers != null)
				MenuOptionsShowLineNumbers.Checked = _editorSettings.ShowLineNumbers;
			if (MenuOptionsShowNewLines != null)
				MenuOptionsShowNewLines.Checked = _editorSettings.ShowEOLMarker;
			if (MenuOptionsHighlightCurrRow != null)
				MenuOptionsHighlightCurrRow.Checked = _editorSettings.LineViewerStyle == LineViewerStyle.FullRow;
			if (MenuOptionsBracketMatchingStyle != null)
				MenuOptionsBracketMatchingStyle.Checked = _editorSettings.BracketMatchingStyle == BracketMatchingStyle.After;
			if (MenuOptionsEnableVirtualSpace != null)
				MenuOptionsEnableVirtualSpace.Checked = _editorSettings.AllowCaretBeyondEOL;*/
        }

        void editor_ActiveTextAreaControl_TextArea_SelectionManager_SelectionChanged(object sender, EventArgs e)
        {
            SelectionManager selMan = sender as SelectionManager;

            if (selMan.HasSomethingSelected)
                UpdateState(EAction.SelectionON);
            else
                UpdateState(EAction.SelectionOFF);
        }

        private void SetModifiedFlag(NthStudio.Gui.Widgets.TextEditor.TextEditor editor, bool flag)
        {
            if (IsModified(editor) != flag)
            {
                TabStripItem p = (TabStripItem)editor.Parent;
                if (IsModified(editor))
                    p.Title = p.Title.Substring(0, p.Title.Length - 1);
                else
                    p.Title += "*";

                UpdateState(EAction.TextChanged);
            }
        }

        /// <summary>Gets whether the file in the specified editor is modified.</summary>
        /// <remarks>TextEditorControl doesn't maintain its own internal modified
        /// flag, so we use the '*' shown after the file name to represent the
        /// modified state.</remarks>
        public bool IsModified(NthStudio.Gui.Widgets.TextEditor.TextEditor editor)
        {
            if (editor == null)
                return false;
            // TextEditorControl doesn't seem to contain its own 'modified' flag, so
            // instead we'll treat the "*" on the filename as the modified flag.

            return ((TabStripItem)editor.Parent).Title.EndsWith("*", StringComparison.InvariantCulture);
        }

        private void RemoveTextEditor(NthStudio.Gui.Widgets.TextEditor.TextEditor editor)
        {
            TabStripItem find = null;

            foreach (TabStripItem tabItem in Items)
            {
                foreach (object o in tabItem.Widgets)
                {
                    NthStudio.Gui.Widgets.TextEditor.TextEditor tec = o as NthStudio.Gui.Widgets.TextEditor.TextEditor;
                    if (tec != null && tec == editor)
                    {
                        find = tabItem;
                        break;
                    }
                }
            }

            if (find != null)
                this.RemoveTab(find);
        }

        public void OpenFiles(string[] fns)
        {
            DoOpenFiles(fns);
            UpdateState(EAction.Loaded);
        }

        void DoOpenFiles(string[] fns)
        {
            // Close default untitled document if it is still empty
            if (Items.Count == 1
                && ActiveEditor.Document.TextLength == 0
                && string.IsNullOrEmpty(ActiveEditor.FileName))
            {
                RemoveTextEditor(ActiveEditor);
            }

            bool continue2 = false;

            // Open file(s)
            foreach (string fn in fns)
            {
                // check if file loaded
                foreach (TabStripItem tabItem in Items)
                {
                    if (Path.GetFullPath((string)tabItem.Tag) == Path.GetFullPath(fn))
                    {
                        //MessageBox.Show("File " + fn + " already loaded ...", "File loaded");
                        continue2 = true;
                        break;
                    }
                }

                if (continue2)
                {
                    continue2 = false;
                    continue;
                }

                NthStudio.Gui.Widgets.TextEditor.TextEditor editor = AddNewTextEditor(fn);
                try
                {
                    editor.LoadFile(fn);
                    // Modified flag is set during loading because the document
                    // "changes" (from nothing to something). So, clear it again.
                    SetModifiedFlag(editor, false);

                    editor.Tag = fn;
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message, ex.GetType().Name);
                    RemoveTextEditor(editor);
                    return;
                }

                // ICSharpCode.TextEditor doesn't have any built-in code folding
                // strategies, so I've included a simple one. Apparently, the
                // foldings are not updated automatically, so in this demo the user
                // cannot add or remove folding regions after loading the file.
                editor.Document.FoldingManager.FoldingStrategy = new RegionFoldingStrategy();
                editor.Document.FoldingManager.UpdateFoldings(null, null);

                // Regions folded by default
                FoldsCollapseExpand(editor, true, true);
            }
        }

        void UpdateState(EAction action)
        {
        }

        void FoldsCollapseExpand(TextEditorBase tec, bool collapse, bool onlyRegions)
        {
            tec.BeginUpdate();

            var lf = new List<FoldMarker>();
            foreach (FoldMarker fm in tec.Document.FoldingManager.FoldMarker)
            {
                if (fm.FoldType == FoldType.Region && onlyRegions)
                {
                    fm.IsFolded = collapse;
                    lf.Add(fm);
                }
                else if (onlyRegions == false)
                {
                    fm.IsFolded = collapse;
                    lf.Add(fm);
                }
            }
            tec.Document.FoldingManager.UpdateFoldings(lf);

            //tec.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
            tec.OptionsChanged();
            tec.EndUpdate();
        }

    }
}
