using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.UITests.Samples.DialogHost;
using XamlTest;
using Xunit;
using Xunit.Abstractions;

namespace MaterialDesignThemes.UITests.WPF.DialogHost
{
    public class DialogHostTests : TestBase
    {
        public DialogHostTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task OnOpenDialog_OverlayCoversContent()
        {
            await using var recorder = new TestRecorder(App);

            IVisualElement dialogHost = await LoadUserControl<WithCounter>();
            IVisualElement overlay = await dialogHost.GetElement("PART_ContentCoverGrid");

            IVisualElement resultTextBlock = await dialogHost.GetElement("ResultTextBlock");
            await Wait.For(async () => await resultTextBlock.GetText() == "Clicks: 0");

            IVisualElement testOverlayButton = await dialogHost.GetElement("TestOverlayButton");
            await testOverlayButton.Click();
            await Wait.For(async () => await resultTextBlock.GetText() == "Clicks: 1");

            IVisualElement showDialogButton = await dialogHost.GetElement("ShowDialogButton");
            await showDialogButton.Click();

            IVisualElement closeDialogButton = await dialogHost.GetElement("CloseDialogButton");
            await Wait.For(async () => await closeDialogButton.GetIsVisible() == true);

            await testOverlayButton.Click();
            await Wait.For(async () => await resultTextBlock.GetText() == "Clicks: 1");
            await closeDialogButton.Click();

            var retry = new Retry(5, TimeSpan.FromSeconds(5));
            await Wait.For(async () => await overlay.GetVisibility() != Visibility.Visible, retry);
            await testOverlayButton.Click();
            await Wait.For(async () => Assert.Equal("Clicks: 2", await resultTextBlock.GetText()), retry);

            recorder.Success();
        }

        [Fact]
        public async Task OnDialogClosed_WithDefaultButton_UpdatesBoundTextBox()
        {
            await using var recorder = new TestRecorder(App);
            IVisualElement dialogHost = await LoadUserControl<WithDefaultButton>();

            IVisualElement showDialogButton = await dialogHost.GetElement("ShowDialogButton");
            await showDialogButton.Click();

            IVisualElement inputTextBox = await Wait.For(async () => await dialogHost.GetElement("InputTextBox"));
            await Wait.For(async () => await inputTextBox.GetIsKeyboardFocused());

            await inputTextBox.SendInput(new KeyboardInput("123"));
            await inputTextBox.SendInput(new KeyboardInput(Key.Enter));

            await Wait.For(async () => await dialogHost.GetProperty<bool>(Wpf.DialogHost.IsOpenProperty) == false);

            IVisualElement textResultTextBlock = await dialogHost.GetElement("TextResult");

            Assert.Equal("123", await textResultTextBlock.GetText());

            recorder.Success();
        }
    }
}
