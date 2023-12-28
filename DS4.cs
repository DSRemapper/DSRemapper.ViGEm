using DSRemapper.Core;
using DSRemapper.Types;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace DSRemapper.ViGEm
{
    /// <summary>
    /// ViGEm DualShock 4 emulated controller class
    /// </summary>
    [EmulatedController("ViGEm/DS4")]
    public class DS4 : IDSROutputController
    {
        private readonly IDualShock4Controller emuController;
        /// <inheritdoc/>
        public bool IsConnected { get; private set; }
        /// <inheritdoc/>
        public IDSRInputReport State { get; private set; } = new DefaultDSRInputReport(6, 0, 14, 1);
        private readonly DefaultDSROutputReport feedback = new();
        /// <summary>
        /// ViGEm DualShock 4 controller class constructor
        /// </summary>
        public DS4()
        {
            ViGEmClient cli = new();
            emuController = cli.CreateDualShock4Controller();
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
            emuController.FeedbackReceived += EmuController_FeedbackReceived; //there is no other way to get it work
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
            emuController.AutoSubmitReport = false;
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
        private void EmuController_FeedbackReceived(object sender, DualShock4FeedbackReceivedEventArgs e)
        {
            feedback.Weak = e.SmallMotor / 255f;
            feedback.Strong = e.LargeMotor / 255f;
            feedback.Led = new(e.LightbarColor.Red, e.LightbarColor.Green, e.LightbarColor.Blue);
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
                emuController.SetAxisValue(DualShock4Axis.LeftThumbX, (byte)(State.LX.ToSByteAxis() + 128));
                emuController.SetAxisValue(DualShock4Axis.LeftThumbY, (byte)(State.LY.ToSByteAxis() + 128));
                emuController.SetAxisValue(DualShock4Axis.RightThumbX, (byte)(State.RX.ToSByteAxis() + 128));
                emuController.SetAxisValue(DualShock4Axis.RightThumbY, (byte)(State.RY.ToSByteAxis() + 128));

                emuController.SetSliderValue(DualShock4Slider.LeftTrigger, State.LTrigger.ToByteTrigger());
                emuController.SetSliderValue(DualShock4Slider.RightTrigger, State.RTrigger.ToByteTrigger());

                if (State.Up && !State.Left && !State.Down && !State.Right)
                    emuController.SetDPadDirection(DualShock4DPadDirection.North);
                else if (State.Up && !State.Left && !State.Down && State.Right)
                    emuController.SetDPadDirection(DualShock4DPadDirection.Northeast);
                else if (!State.Up && !State.Left && !State.Down && State.Right)
                    emuController.SetDPadDirection(DualShock4DPadDirection.East);
                else if (!State.Up && !State.Left && State.Down && State.Right)
                    emuController.SetDPadDirection(DualShock4DPadDirection.Southeast);
                else if (!State.Up && !State.Left && State.Down && !State.Right)
                    emuController.SetDPadDirection(DualShock4DPadDirection.South);
                else if (!State.Up && State.Left && State.Down && !State.Right)
                    emuController.SetDPadDirection(DualShock4DPadDirection.Southwest);
                else if (!State.Up && State.Left && !State.Down && !State.Right)
                    emuController.SetDPadDirection(DualShock4DPadDirection.West);
                else if (State.Up && State.Left && !State.Down && !State.Right)
                    emuController.SetDPadDirection(DualShock4DPadDirection.Northwest);
                else
                    emuController.SetDPadDirection(DualShock4DPadDirection.None);

                emuController.SetButtonState(DualShock4Button.Cross, State.Cross);
                emuController.SetButtonState(DualShock4Button.Circle, State.Circle);
                emuController.SetButtonState(DualShock4Button.Square, State.Square);
                emuController.SetButtonState(DualShock4Button.Triangle, State.Triangle);

                emuController.SetButtonState(DualShock4Button.Options, State.Options);
                emuController.SetButtonState(DualShock4Button.Share, State.Share);

                emuController.SetButtonState(DualShock4Button.ShoulderLeft, State.L1);
                emuController.SetButtonState(DualShock4Button.ShoulderRight, State.R1);
                emuController.SetButtonState(DualShock4Button.TriggerLeft, State.L2);
                emuController.SetButtonState(DualShock4Button.TriggerRight, State.R2);
                emuController.SetButtonState(DualShock4Button.ThumbLeft, State.L3);
                emuController.SetButtonState(DualShock4Button.ThumbRight, State.R3);

                emuController.SetButtonState(DualShock4SpecialButton.Ps, State.PS);
                emuController.SetButtonState(DualShock4SpecialButton.Touchpad, State.TouchPad);

                emuController.SubmitReport();
            }
        }
    }
}