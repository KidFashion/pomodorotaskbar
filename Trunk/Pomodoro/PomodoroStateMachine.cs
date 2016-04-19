using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stateless;
using Pomodoro.Properties;

namespace Pomodoro
{
    public enum State { DoingWork, DoingRelax};

    public enum Trigger { StartWorking, StopWorking };

    public class PomodoroStateMachine : StateMachine<State, Trigger>
    {
        //private StateMachine<State, Trigger> _sm;

        private void initialize()
        {
            this.Configure(State.DoingWork)
                .Permit(Trigger.StopWorking, State.DoingRelax);

            this.Configure(State.DoingRelax)
                .Permit(Trigger.StartWorking, State.DoingWork);

            _workSeconds = Settings.Default.SessionLength * 60;
            _relaxSeconds = Settings.Default.RelaxLength * 60;

        }

        private int _workSeconds;
        private int _relaxSeconds;


        public void MoveToNextState()
        {
            if (this.State == State.DoingRelax) { this.Fire(Trigger.StartWorking); }
            else this.Fire(Trigger.StopWorking); 
        }

        public int CurrentTime
        {
            get
            {
                if (this.State == State.DoingRelax) { return _relaxSeconds; }
                else return _workSeconds;
            }
        }
        public PomodoroStateMachine() : base(State.DoingWork)
        { }

        public PomodoroStateMachine(State initialState) : base(initialState)
        {
            //this._sm = new StateMachine<State, Trigger>(initialState);
        }

    }
}
