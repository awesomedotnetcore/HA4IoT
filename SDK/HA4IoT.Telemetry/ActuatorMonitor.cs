﻿using System;
using HA4IoT.Actuators;
using HA4IoT.Contracts.Actuators;

namespace HA4IoT.Telemetry
{
    public abstract class ActuatorMonitor
    { 
        public void ConnectActuators(Home home)
        {
            if (home == null) throw new ArgumentNullException(nameof(home));

            foreach (var actuator in home.Actuators.Values)
            {
                OnActuatorConnecting(actuator);

                var binaryStateOutput = actuator as IBinaryStateOutputActuator;
                if (binaryStateOutput != null)
                {
                    HandleBinaryStateOutputActuator(binaryStateOutput);
                    continue;
                }

                var stateMachineOutput = actuator as StateMachine;
                if (stateMachineOutput != null)
                {
                    HandleStateMachineOutputActuator(stateMachineOutput);
                    continue;
                }

                var sensor = actuator as ISingleValueSensorActuator;
                if (sensor != null)
                {
                    OnSensorValueChanged(sensor, sensor.GetValue());
                    sensor.ValueChanged += (s, e) =>
                    {
                        OnSensorValueChanged(sensor, e.NewValue);
                    };

                    continue;
                }

                var motionDetector = actuator as IMotionDetector;
                if (motionDetector != null)
                {
                    motionDetector.MotionDetected += (s, e) =>
                    {
                        OnMotionDetected(motionDetector);
                    };

                    continue;
                }

                var button = actuator as IButton;
                if (button != null)
                {
                    button.PressedShort += (s, e) =>
                    {
                        OnButtonPressed(button, ButtonPressedDuration.Short);
                    };

                    button.PressedLong += (s, e) =>
                    {
                        OnButtonPressed(button, ButtonPressedDuration.Long);
                    };
                }
            }
        }

        protected virtual void OnActuatorConnecting(IActuator actuator)
        {
        }

        protected virtual void OnMotionDetected(IMotionDetector motionDetector)
        {
        }

        protected virtual void OnButtonPressed(IButton button, ButtonPressedDuration duration)
        {
        }

        protected virtual void OnBinaryStateActuatorStateChanged(IBinaryStateOutputActuator actuator, BinaryActuatorState newState)
        {
        }

        protected virtual void OnStateMachineStateChanged(StateMachine stateMachine, string newState)
        {
        }

        protected virtual void OnSensorValueChanged(ISingleValueSensorActuator sensor, float newValue)
        {
        }

        private void HandleBinaryStateOutputActuator(IBinaryStateOutputActuator binaryStateOutputActuator)
        {
            OnBinaryStateActuatorStateChanged(binaryStateOutputActuator, binaryStateOutputActuator.GetState());
            
            binaryStateOutputActuator.StateChanged += (s, e) =>
            {
                OnBinaryStateActuatorStateChanged(binaryStateOutputActuator, e.NewValue);
            };
        }

        private void HandleStateMachineOutputActuator(StateMachine stateMachine)
        {
            OnStateMachineStateChanged(stateMachine, stateMachine.GetState());

            stateMachine.StateChanged += (s, e) =>
            {
                OnStateMachineStateChanged(stateMachine, e.NewValue);
            };
        }
    }
}