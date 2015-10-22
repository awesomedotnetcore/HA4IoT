﻿using System;

namespace CK.HomeAutomation.Hardware.RemoteSwitch
{
    public class RemoteSwitchOutputPort : IBinaryOutput
    {
        private readonly LPD433MHzCodeSequence _onCodeSqCodeSequence;
        private readonly LPD433MHzCodeSequence _offCodeSequence;
        private readonly LPD433MHzSignalSender _sender;
        private readonly object _syncRoot = new object();
        private BinaryState _state;

        public RemoteSwitchOutputPort(int id, LPD433MHzCodeSequence onCodeSequence, LPD433MHzCodeSequence offCodeSequence, LPD433MHzSignalSender sender)
        {
            if (onCodeSequence == null) throw new ArgumentNullException(nameof(onCodeSequence));
            if (offCodeSequence == null) throw new ArgumentNullException(nameof(offCodeSequence));
            if (sender == null) throw new ArgumentNullException(nameof(sender));

            _onCodeSqCodeSequence = onCodeSequence;
            _offCodeSequence = offCodeSequence;
            _sender = sender;
        }

        public void Write(BinaryState state, bool commit = true)
        {
            if (commit == false)
            {
                throw new NotSupportedException();
            }

            lock (_syncRoot)
            {
                if (state == BinaryState.High)
                {
                    _sender.Send(_onCodeSqCodeSequence);
                }
                else if (state == BinaryState.Low)
                {
                    _sender.Send(_offCodeSequence);
                }
                else
                {
                    throw new NotSupportedException();
                }

                _state = state;
            }
        }

        public BinaryState Read()
        {
            lock (_syncRoot)
            {
                return _state;
            }
        }

        public IBinaryOutput WithInvertedState()
        {
            throw new NotSupportedException();
        }
    }
}