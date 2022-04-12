Imports Windows.UI.ViewManagement.Core

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub BoldButton_Click(sender As Object, e As RoutedEventArgs) Handles BoldButton.Click
        Dim selectedText As Windows.UI.Text.ITextSelection = PostPadBox.Document.Selection
        If selectedText IsNot Nothing Then
            Dim charFormatting As Windows.UI.Text.ITextCharacterFormat = selectedText.CharacterFormat
            charFormatting.Bold = Windows.UI.Text.FormatEffect.Toggle
            selectedText.CharacterFormat = charFormatting
        End If
    End Sub
    Private Sub ItalicButton_Click(sender As Object, e As RoutedEventArgs) Handles ItalicButton.Click
        Dim selectedText As Windows.UI.Text.ITextSelection = PostPadBox.Document.Selection
        If selectedText IsNot Nothing Then
            Dim charFormatting As Windows.UI.Text.ITextCharacterFormat = selectedText.CharacterFormat
            charFormatting.Italic = Windows.UI.Text.FormatEffect.Toggle
            selectedText.CharacterFormat = charFormatting
        End If
    End Sub
    Private Sub UnderlineButton_Click(sender As Object, e As RoutedEventArgs) Handles UnderlineButton.Click
        Dim selectedText As Windows.UI.Text.ITextSelection = PostPadBox.Document.Selection
        If selectedText IsNot Nothing Then
            Dim charFormatting As Windows.UI.Text.ITextCharacterFormat = selectedText.CharacterFormat
            If charFormatting.Underline = Windows.UI.Text.UnderlineType.None Then
                charFormatting.Underline = Windows.UI.Text.UnderlineType.Single
            Else
                charFormatting.Underline = Windows.UI.Text.UnderlineType.None
            End If
            selectedText.CharacterFormat = charFormatting
        End If
    End Sub
    Private Sub StrikeoutButton_Click(sender As Object, e As RoutedEventArgs) Handles StrikeoutButton.Click
        Dim selectedText As Windows.UI.Text.ITextSelection = PostPadBox.Document.Selection
        If selectedText IsNot Nothing Then
            Dim charFormatting As Windows.UI.Text.ITextCharacterFormat = selectedText.CharacterFormat
            charFormatting.Strikethrough = Windows.UI.Text.FormatEffect.Toggle
            selectedText.CharacterFormat = charFormatting
        End If
    End Sub
    Private Sub BulletButton_Click(sender As Object, e As RoutedEventArgs) Handles BulletButton.Click
        PostPadBox.Document.Selection.ParagraphFormat.ListType = Windows.UI.Text.MarkerType.Bullet
    End Sub
    Private Async Sub ImageButton_Click(sender As Object, e As RoutedEventArgs) Handles ImageButton.Click
        Dim open As Windows.Storage.Pickers.FileOpenPicker = New Windows.Storage.Pickers.FileOpenPicker()
        open.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
        open.FileTypeFilter.Add(".jpg")
        open.FileTypeFilter.Add(".gif")
        open.FileTypeFilter.Add(".png")
        Dim file As Windows.Storage.StorageFile = Await open.PickSingleFileAsync()
        If file IsNot Nothing Then
            Dim RANDOME As Windows.Storage.Streams.IRandomAccessStream = Await file.OpenAsync(Windows.Storage.FileAccessMode.Read)
            Dim image As BitmapImage = New BitmapImage()
            PostPadBox.Document.Selection.InsertImage(image.PixelWidth, image.PixelHeight, 0, Windows.UI.Text.VerticalCharacterAlignment.Baseline, file.DisplayName.ToString(), RANDOME)
        End If
        If file Is Nothing Then
            Dim errorDialog As ContentDialog = New ContentDialog() With {
                .Title = "Open File Error",
                .Content = "Sorry, the selected file could not be opened. Please hang up and try again.",
                .CloseButtonText = "Ok"
            }
            Await errorDialog.ShowAsync()
        End If
    End Sub
    Private Sub UndoButton_Click(sender As Object, e As RoutedEventArgs) Handles UndoButton.Click
        If PostPadBox.Document.CanUndo Then
            PostPadBox.Document.Undo()
        End If
    End Sub
    Private Sub RedoButton_Click(sender As Object, e As RoutedEventArgs) Handles RedoButton.Click
        If PostPadBox.Document.CanRedo Then
            PostPadBox.Document.Redo()
        End If
    End Sub
    Private Async Sub SaveButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles SaveButton.Click
        Dim savePicker As Windows.Storage.Pickers.FileSavePicker = New Windows.Storage.Pickers.FileSavePicker()
        savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
        savePicker.FileTypeChoices.Add("Text document", New List(Of String)() From {".txt"})
        savePicker.SuggestedFileName = "PostPad document"
        Dim file As Windows.Storage.StorageFile = Await savePicker.PickSaveFileAsync()
        If file IsNot Nothing Then
            Windows.Storage.CachedFileManager.DeferUpdates(file)
            Dim randAccStream As Windows.Storage.Streams.IRandomAccessStream = Await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite)
            PostPadBox.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream)
            Dim status As Windows.Storage.Provider.FileUpdateStatus = Await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file)
            If status <> Windows.Storage.Provider.FileUpdateStatus.Complete Then
                Dim errorBox As Windows.UI.Popups.MessageDialog = New Windows.UI.Popups.MessageDialog("*Sniffs* Sorry, file '" & file.DisplayName & "' couldn't be saved...boo-hoo-hoo! :,(")
                Await errorBox.ShowAsync()
            End If
        End If
    End Sub
    Private Sub TrashButton_Click(sender As Object, e As RoutedEventArgs) Handles TrashButton.Click
        PostPadBox.Document.SetText(Windows.UI.Text.TextSetOptions.None, "")
    End Sub
    Private Sub PostButton_Click(sender As Object, e As RoutedEventArgs) Handles PostButton.Click
        PostPadBox.Focus(FocusState.Pointer)
        PostPadBox.Document.Selection.SetRange(0, PostPadBox.Document.Selection.EndPosition)
        PostPadListbox.Items.Add(PostPadBox.Document.Selection.Text.ToString() & ", published " & DateTime.Now.ToString("dddd, M/d/yyyy, h:mm tt"))
        NotesHeader.Content = "So far, you have " & PostPadListbox.Items.Count - 1 & " notes."
        If PostPadListbox.Items.Count = 2 Then
            NotesHeader.Content = "So far, you have only 1 note."
        End If
        If PostPadListbox.Items.Count = 1 Then
            NotesHeader.Content = "So far, you have no notes. ☁️😌"
        End If
    End Sub
    Private Sub PostPadBox_TextChanged(sender As Object, e As RoutedEventArgs) Handles PostPadBox.TextChanged
        If PostPadBox.Document.CanUndo Then
            UndoButton.IsEnabled = True
        Else
            UndoButton.IsEnabled = False
        End If
        If PostPadBox.Document.CanRedo Then
            RedoButton.IsEnabled = True
        Else
            RedoButton.IsEnabled = False
        End If
    End Sub
    Private Sub SetButton_Click(sender As Object, e As RoutedEventArgs) Handles SetButton.Click
        PostPadBox.Document.Selection.Text = PostPadListbox.SelectedItem.ToString()
    End Sub
    Private Async Sub DeleteNoteButton_Click(sender As Object, e As RoutedEventArgs) Handles DeleteNoteButton.Click
        Dim deleteDialog As ContentDialog = New ContentDialog() With {
            .Title = "Delete note?",
            .Content = "Do you want to delete this note?",
            .PrimaryButtonText = "Yes",
            .CloseButtonText = "No"
        }
        Dim result As ContentDialogResult = Await deleteDialog.ShowAsync()
        If result = ContentDialogResult.Primary Then
            PostPadListbox.Items.Remove(PostPadListbox.SelectedItem)
            NotesHeader.Content = "So far, you have " & PostPadListbox.Items.Count - 1 & " notes."
            If PostPadListbox.Items.Count = 2 Then
                NotesHeader.Content = "So far, you have only 1 note."
            End If
            If PostPadListbox.Items.Count = 1 Then
                NotesHeader.Content = "So far, you have no notes. ☁️😌"
            End If
        End If
    End Sub
    Private Sub EmojiButton_Click(sender As Object, e As RoutedEventArgs) Handles EmojiButton.Click
        CoreInputView.GetForCurrentView().TryShow(CoreInputViewKind.Emoji)
    End Sub
    Private Async Sub OpenButton_Click(sender As Object, e As RoutedEventArgs) Handles OpenButton.Click
        Dim open As Windows.Storage.Pickers.FileOpenPicker = New Windows.Storage.Pickers.FileOpenPicker()
        open.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
        open.FileTypeFilter.Add(".txt")
        Dim file As Windows.Storage.StorageFile = Await open.PickSingleFileAsync()
        If file IsNot Nothing Then
            Dim randAccStream As Windows.Storage.Streams.IRandomAccessStream = Await file.OpenAsync(Windows.Storage.FileAccessMode.Read)
            PostPadBox.Document.LoadFromStream(Windows.UI.Text.TextSetOptions.FormatRtf, randAccStream)
        End If
        If file Is Nothing Then
            Dim errorDialog As ContentDialog = New ContentDialog() With {
                .Title = "Open File Error",
                .Content = "Sorry, the selected file could not be opened. Please hang up and try again.",
                .CloseButtonText = "Ok"
            }
            Await errorDialog.ShowAsync()
        End If
    End Sub
    Private Sub PrayerBox_KeyDown(sender As Object, e As KeyRoutedEventArgs) Handles PrayerBox.KeyDown
        If e.Key = Windows.System.VirtualKey.Enter Then
            PostPadBox.Document.SetText(Windows.UI.Text.TextSetOptions.None, "Dear Jesus, I pray for " & PrayerBox.Text.ToString() & ". May You richly bless that, Lord. Amen.")
        End If
    End Sub
End Class