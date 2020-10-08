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

        string state = InitialState;
        Tape tape = null;
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
                return state.StartsWith("halt");
            }
        }

        public Tape Tape
        {
            get
            {
                return tape;
            }
        }

        public string State
        {
            get
            {
                return state;
            }
        }

        /// <summary>
        /// Reset the status of the machine and prepare it to execute a new program
        /// </summary>
        /// <remarks>
        /// Doesn't throw events even if the machine's internal variables change
        /// </remarks>
        public void Reset(string input)
        {
            state = InitialState;
            tape = new Tape();
            if (input != null)
                tape.InitializeWithData(input);
        }

        public Statement Step()
        {
            if (IsHalted)
                throw new InvalidOperationException("Machine is in halted state");

            var statement = SelectNextStatement();

            // No rule for machine's current state and symbol
            if (statement == null)
                throw new IncompleteProgramException()
                {
                    CurrentSymbol = tape.CurrentSymbol,
                    State = state
                };

            // Execute statement
            state = statement.NewState;
            OnStateChanged(new MachineStateChangedEventArgs(state, statement.NewState));

            if (statement.NewSymbol != Statement.AnySymbol)
                tape.CurrentSymbol = statement.NewSymbol;

            if (statement.Direction != TapeDirection.Still)
            {
                if (statement.Direction == TapeDirection.Right)
                    tape.MoveRight();
                else
                    tape.MoveLeft();
            }

            OnTapeChanged(new TapeChangedEventArgs() { OldSymbol = tape.CurrentSymbol, NewSymbol = statement.NewSymbol, Direction = statement.Direction });

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
                .Where(s => s.CurrentState == state && (s.CurrentSymbol == tape.CurrentSymbol || s.CurrentSymbol == Statement.AnySymbol))
                .FirstOrDefault();
        }

        #region Events

        protected virtual void OnStateChanged(MachineStateChangedEventArgs e)
        {
            if (StateChanged != null)
                StateChanged(this, e);
        }

        protected void OnStateChanged()
        {

        }

        public virtual void OnTapeChanged(TapeChangedEventArgs e)
        {
            if (TapeChanged != null)
                TapeChanged(this, e);
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
