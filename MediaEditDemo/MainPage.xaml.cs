using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Editing;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MediaEditDemo
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaStreamSource mediaStreamSource;
        private StorageFile firstVideoFile;
        private StorageFile secondVideoFile;
        private MediaComposition composition;
        public MainPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            mediaElement.Source = null;
            mediaStreamSource = null;
            base.OnNavigatedFrom(e);
        }

        private async void ChooseFirstVideo_Click(object sender, RoutedEventArgs e)
        {
            // Get first file
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            picker.FileTypeFilter.Add(".mp4");
            firstVideoFile = await picker.PickSingleFileAsync();
            if (firstVideoFile == null)
            {
                //rootPage.NotifyUser("File picking cancelled", NotifyType.ErrorMessage);
                return;
            }

            mediaElement.SetSource(await firstVideoFile.OpenReadAsync(), firstVideoFile.ContentType);
            chooseSecondFile.IsEnabled = true;

        }

        private async void ChooseSecondVideo_Click(object sender, RoutedEventArgs e)
        {

            // Get second file
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            picker.FileTypeFilter.Add(".mp4");
            secondVideoFile = await picker.PickSingleFileAsync();
            if (secondVideoFile == null)
            {
                //rootPage.NotifyUser("File picking cancelled", NotifyType.ErrorMessage);
                return;
            }

            mediaElement.SetSource(await secondVideoFile.OpenReadAsync(), secondVideoFile.ContentType);
            appendFiles.IsEnabled = true;
        }

        private async void AppendVideos_Click(object sender, RoutedEventArgs e)
        {
            // Combine two video files together into one
            var firstClip = await MediaClip.CreateFromFileAsync(firstVideoFile);
            var secondClip = await MediaClip.CreateFromFileAsync(secondVideoFile);

            composition = new MediaComposition();
            composition.Clips.Add(firstClip);
            composition.Clips.Add(secondClip);

            // Render to MediaElement.
            mediaElement.Position = TimeSpan.Zero;
            mediaStreamSource = composition.GeneratePreviewMediaStreamSource((int)mediaElement.ActualWidth, (int)mediaElement.ActualHeight);
            mediaElement.SetMediaStreamSource(mediaStreamSource);
            //rootPage.NotifyUser("Clips appended", NotifyType.StatusMessage);
        }
    }
}
