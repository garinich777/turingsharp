﻿using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TuringSharp.Parser;
using TuringSharp.Runtime;

namespace TuringSharp.UI
{
    public partial class frmMain : Form
    {
        Machine machine;
        Task machineTask;
        CancellationTokenSource machineTaskCancellationToken;
        bool isRun = false;
        
        public frmMain()
        {
            //btnStep. = true;
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            ResetMachine(null);
        }

        private void ResetMachine(string input)
        {
            machine = new Machine();
            machine.Reset(input);
            machine.StateChanged += Machine_StateChanged;
            machine.TapeChanged += Machine_TapeChanged;
            UpdateTape();
            UpdateCurrentState();
        }

        private void Machine_TapeChanged(object arg1, Machine.TapeChangedEventArgs arg2)
        {
            UpdateTape();
        }

        private void Machine_StateChanged(object arg1, Machine.MachineStateChangedEventArgs arg2)
        {
            UpdateCurrentState();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (ofdSelectFile.ShowDialog() == DialogResult.OK)
            {
                rtbCode.Text = File.ReadAllText(ofdSelectFile.FileName);
            }
        }

        private void UpdateCurrentState()
        {
            InvokeOnMainThread(() =>
            {
                txtCurrentState.Text = machine.State;
                txtSteps.Text = machine.StepsNumber.ToString();
            });
        }

        private void UpdateTape()
        {
            InvokeOnMainThread(() =>
            {
                const int kNumberOfSymbols = 18; // Number of symbols to read from each side with respect to the currently-pointed symbol.
                string tapeContent = machine.Tape.GetCellsAsString(kNumberOfSymbols, kNumberOfSymbols);

                rtbTape.ResetText();
                rtbTape.Text = tapeContent;
                rtbTape.SelectAll();
                rtbTape.SelectionAlignment = HorizontalAlignment.Center;
                rtbTape.DeselectAll();

                // Highlight current symbol
                rtbTape.Select(kNumberOfSymbols, 1);
                rtbTape.SelectionBackColor = Color.DarkGreen;
                rtbTape.SelectionColor = Color.NavajoWhite;
                rtbTape.DeselectAll();
            });
        }

        private void InvokeOnMainThread(Action toRun)
        {
            if (InvokeRequired)
                Invoke(toRun);
            else
                toRun.Invoke();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            EnterRunMode();
            if (!isRun)
            {
                isRun = true;
                try
                {
                    var program = new ProgramParser().Parse(rtbCode.Text);

                    // Check if the program contains any executable statement
                    if (program.Statements.Count == 0)
                    {
                        MessageBox.Show("Program is empty");
                        ExitRunMode();
                        return;
                    }

                    var input = txtInput.Text.Replace(' ', Tape.Blank);
                    if (input.Length == 0)
                        input = null;

                    machine.Load(program, input);
                }
                catch (ParserException ex)
                {
                    // Don't leave the machine in a unconsistent state
                    ResetMachine(null);

                    if (ex.LineNumber.HasValue)
                        MessageBox.Show(string.Format("Error while parsing the program: {0} on line {1}", ex.Message, ex.LineNumber.Value));
                    else
                        MessageBox.Show(string.Format("Error while parsing the program: {0}", ex.Message));

                    ExitRunMode();
                }
            }

            if (chkRunInFullSpeed.Checked)
            {
                machine.Run();
                UpdateCurrentState();
                ExitRunMode();
            }
            else
            {
                machineTaskCancellationToken = new CancellationTokenSource();
                var ct = machineTaskCancellationToken.Token;
                machineTask = Task.Factory.StartNew(() =>
                {
                    while (!machine.IsHalted && !ct.IsCancellationRequested)
                    {
                        machine.Step();
                        UpdateCurrentState();
                        Thread.Sleep(150);
                    }
                    InvokeOnMainThread(() => ExitRunMode());
                    if (!ct.IsCancellationRequested)
                       isRun = false;
                }, ct);
            }
            isRun = !machine.IsHalted;
        }

        private void EnterRunMode()
        {
            btnRun.Enabled = false;
            chkRunInFullSpeed.Enabled = false;
            btnStep.Enabled = false;
            btnReset.Enabled = false;
            if (!chkRunInFullSpeed.Checked)
                btnPause.Enabled = true;
        }

        private void ExitRunMode()
        {
            btnRun.Enabled = true;
            chkRunInFullSpeed.Enabled = true;
            btnStep.Enabled = true;
            btnPause.Enabled = false;
            btnReset.Enabled = true;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            machineTaskCancellationToken.Cancel();
            ExitRunMode();
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            if (machine.IsHalted)
                MessageBox.Show("Machine is in halted state");
            else
                machine.Step();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            isRun = false;
            ResetMachine(null);
            ExitRunMode();
        }
    }
}
