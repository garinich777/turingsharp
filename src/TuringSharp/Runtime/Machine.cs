using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSharp.CodeDom;

namespace TuringSharp.Runtime
{
    /// <summary>
    /// Represents a turing machine
    /// </summary>
    public class Machine
    {
        public const string InitialState = "0";

        public event Action<object, MachineStateChangedEventArgs> StateChanged;
        public event Action<object, TapeChangedEventArgs> TapeChanged;

        Program program = null;

        public void Load(Program p, string input)
        {
            program = p;
            Reset(input);
        }

        public bool IsHalted
        {
            get
            {
                return State.StartsWith("halt");
            }
        }

        public int StepsNumber { get; private set; } = 0;

        public Tape Tape { get; private set; } = null;

        public string State { get; private set; } = InitialState;

        /// <summary>
        /// Reset the status of the machine and prepare it to execute a new program
        /// </summary>
        /// <remarks>
        /// Doesn't throw events even if the machine's internal variables change
        /// </remarks>
        public void Reset(string input)
        {
            State = InitialState;
            Tape = new Tape();
            if (input != null)
                Tape.InitializeWithData(input);
        }

        public Statement Step()
        {
            if (IsHalted)
                throw new InvalidOperationException("Machine is in halted state");

            var statement = SelectNextStatement();

            // No rule for machine's current state and symbol
            if (statement == null)
            {
                throw new IncompleteProgramException()
                {
                    CurrentSymbol = Tape.CurrentSymbol,
                    State = State
                };
            }

            // Execute statement
            State = statement.NewState;
            OnStateChanged(new MachineStateChangedEventArgs(State, statement.NewState));

            if (statement.NewSymbol != Statement.AnySymbol)
                Tape.CurrentSymbol = statement.NewSymbol;

            if (statement.Direction != TapeDirection.Still)
            {
                if (statement.Direction == TapeDirection.Right)
                    Tape.MoveRight();
                else
                    Tape.MoveLeft();
            }

            OnTapeChanged(new TapeChangedEventArgs() { OldSymbol = Tape.CurrentSymbol, NewSymbol = statement.NewSymbol, Direction = statement.Direction });

            StepsNumber++;
            // Return the statement executed
            return statement;
        }

        /// <summary>
        /// Run the machine until it halts
        /// </summary>
        public void Run()
        {
            while (!IsHalted)
                Step();
        }

        /// <summary>
        /// Select the next statement to run based on the current state and the current symbol on the tape's head.
        /// Gives precedence to more specific statements.
        /// </summary>
        public Statement SelectNextStatement()
        {
            return program.Statements
                .OrderBy(s => s.CurrentSymbol == Statement.AnySymbol) // Give precedence to specific rules first
                .Where(s => s.CurrentState == State && (s.CurrentSymbol == Tape.CurrentSymbol || s.CurrentSymbol == Statement.AnySymbol))
                .FirstOrDefault();
        }

        #region Events

        protected virtual void OnStateChanged(MachineStateChangedEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }

        protected void OnStateChanged()
        {

        }

        public virtual void OnTapeChanged(TapeChangedEventArgs e)
        {
            TapeChanged?.Invoke(this, e);
        }

        public class MachineStateChangedEventArgs : EventArgs
        {

            public MachineStateChangedEventArgs(string oldState, string newState)
            {
                OldState = oldState;
                NewState = newState;
            }

            public string OldState { get; set; }

            public string NewState { get; set; }

        }

        public class TapeChangedEventArgs : EventArgs
        {
            public TapeChangedEventArgs()
            {
            }

            public char OldSymbol { get; set; }

            public char NewSymbol { get; set; }

            public TapeDirection Direction { get; set; }
        }

        #endregion
    }

}
