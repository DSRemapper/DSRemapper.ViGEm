using DSRemapper.Core;
using DSRemapper.Types;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DSRemapper.ViGEm
{
    /// <summary>
    /// ViGEm Xbox emulated controller class
    /// </summary>
    [EmulatedController("ViGEm/Xbox")]
    public class Xbox : IDSROutputController
    {
        private readonly IXbox360Controller emuController;
        /// <inheritdoc/>
        public bool IsConnected { get; private set; }
        /// <inheritdoc/>
        public IDSRInputReport State { get; set; } = new DefaultDSRInputReport(6,0,14,1,0);
        private readonly DefaultDSROutputReport feedback = new();
        /// <summary>
        /// ViGEm Xbox controller class constructor
        /// </summary>
        public Xbox()
        {
            ViGEmClient cli = new();
            emuController=cli.CreateXbox360Controller();
            emuController.FeedbackReceived += EmuController_FeedbackReceived;
            emuController.AutoSubmitReport = true;
        }
        /// <inheritdoc/>
        public void Connect()
        {
            if (!IsConnected)
            {
                emuController.Connect();
                IsConnected = true;
            }
        }
        /// <inheritdoc/>
        public void Disconnect()
        {
            if (IsConnected)
            {
                emuController.Disconnect();
                IsConnected = false;
            }
        }
        /// <inheritdoc/>
        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }
        private void EmuController_FeedbackReceived(object sender, Xbox360FeedbackReceivedEventArgs e)
        {
            feedback.Weak = e.SmallMotor / 255f;
            feedback.Strong = e.LargeMotor / 255f;
            switch (feedback.Led.Player = e.LedNumber)
            {
                case 0:
                    feedback.Led.SetRGB(0, 0, 1);
                    break;
                case 1:
                    feedback.Led.SetRGB(1, 0, 0);
                    break;
                case 2:
                    feedback.Led.SetRGB(0, 1, 0);
                    break;
                case 3:
                    feedback.Led.SetRGB(1, 0, 1);
                    break;
            }
        }
        /// <inheritdoc/>
        public DefaultDSROutputReport GetFeedbackReport()
        {
            return feedback;
        }
        /// <inheritdoc/>
        public void Update()
        {
            if (IsConnected)
            {
                emuController.SetAxisValue(Xbox360Axis.LeftThumbX, State.LX.ToShortAxis());
                emuController.SetAxisValue(Xbox360Axis.LeftThumbY, (-State.LY).ToShortAxis());
                emuController.SetAxisValue(Xbox360Axis.RightThumbX, State.RX.ToShortAxis());
                emuController.SetAxisValue(Xbox360Axis.RightThumbY, (-State.RY).ToShortAxis());

                emuController.SetSliderValue(Xbox360Slider.LeftTrigger, State.LTrigger.ToByteTrigger());
                emuController.SetSliderValue(Xbox360Slider.RightTrigger, State.RTrigger.ToByteTrigger());

                emuController.SetButtonState(Xbox360Button.A, State.A);
                emuController.SetButtonState(Xbox360Button.B, State.B);
                emuController.SetButtonState(Xbox360Button.X, State.X);
                emuController.SetButtonState(Xbox360Button.Y, State.Y);

                emuController.SetButtonState(Xbox360Button.Up, State.Up);
                emuController.SetButtonState(Xbox360Button.Left, State.Left);
                emuController.SetButtonState(Xbox360Button.Down, State.Down);
                emuController.SetButtonState(Xbox360Button.Right, State.Right);

                emuController.SetButtonState(Xbox360Button.LeftShoulder, State.LButton);
                emuController.SetButtonState(Xbox360Button.RightShoulder, State.RButton);
                emuController.SetButtonState(Xbox360Button.LeftThumb, State.LThumb);
                emuController.SetButtonState(Xbox360Button.RightThumb, State.RThumb);

                emuController.SetButtonState(Xbox360Button.Back, State.Back);
                emuController.SetButtonState(Xbox360Button.Start, State.Start);
                emuController.SetButtonState(Xbox360Button.Guide, State.Guide);
                emuController.SubmitReport();
            }
        }
    }
}
