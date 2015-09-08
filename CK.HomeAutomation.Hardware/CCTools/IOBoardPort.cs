﻿using System;
using CK.HomeAutomation.Core;

namespace CK.HomeAutomation.Hardware.CCTools
{
    public class IOBoardPort : IBinaryInput, IBinaryOutput
    {
        public IOBoardPort(int number, IOBoardController controller)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));

            Number = number;
            Controller = controller;

            controller.StateChanged += FireEvents;
        }

        public event EventHandler<BinaryStateChangedEventArgs> StateChanged; 

        public int Number { get; }
        public bool ValueIsInverted { get; set; }
        public IOBoardController Controller { get; }

        public BinaryState Read()
        {
            return CoerceState(Controller.GetPortState(Number));
        }

        public void Write(BinaryState state, bool commit)
        {
            state = CoerceState(state);
            Controller.SetPortState(Number, state);

            if (commit)
            {
                Controller.CommitChanges();
            }
        }

        IBinaryOutput IBinaryOutput.WithInvertedState()
        {
            ValueIsInverted = true;
            return this;
        }

        IBinaryInput IBinaryInput.WithInvertedState()
        {
            ValueIsInverted = true;
            return this;
        }

        public IOBoardPort WithInvertedState()
        {
            ValueIsInverted = true;
            return this;
        }

        private BinaryState CoerceState(BinaryState state)
        {
            if (ValueIsInverted)
            {
                return state == BinaryState.High ? BinaryState.Low : BinaryState.High;
            }

            return state;
        }

        private void FireEvents(object sender, IOBoardStateChangedEventArgs e)
        {
            bool oldState = e.OldState.GetBit(Number);
            bool newState = e.NewState.GetBit(Number);

            if (oldState == newState)
            {
                return;
            }

            BinaryState state = Read();
            StateChanged?.Invoke(this, new BinaryStateChangedEventArgs(state));
        }
    }
}