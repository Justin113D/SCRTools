using SCR.Tools.Dialog.Data.Simulation;
using SCR.Tools.WPF.Viewmodeling;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static SCR.Tools.UndoRedo.GlobalChangeTrackerC;

namespace SCR.Tools.Dialog.Simulator.Viewmodeling
{
    internal class VmSimulator : BaseViewModel
    {
        private VmPathChoice _selectedPathChoice;

        private readonly DialogSimulator _simulator;

        public VmMain Main { get; }


        public VmPathChoice[] PathChoices { get; }

        public VmPathChoice SelectedPathChoice
        {
            get => _selectedPathChoice;
            [MemberNotNull(nameof(_selectedPathChoice), nameof(Text))]
            set
            {
                _selectedPathChoice = value;
                SelectChoice(Array.IndexOf(PathChoices, value));
            }
        }

        public int SelectedPathChoiceIndex { get; private set; }

        public bool HasMultipleChoices { get; private set; }

        public bool EndReached { get; private set; }


        public string Text { get; private set; }

        public string? LeftPortraitPath { get; private set; }

        public string LeftPortraitLabel { get; private set; }

        public string? RightPortraitPath { get; private set; }

        public string RightPortraitLabel { get; private set; }

        public bool HighlightRightPortrait { get; private set; }


        public RelayCommand CmdReset
            => new(Reset);

        public RelayCommand CmdNext
            => new(Next);

        public RelayCommand CmdUndo
            => new(Undo);

        public RelayCommand CmdRedo
            => new(Redo);


        public VmSimulator(VmMain main)
        {
            Main = main;
            _simulator = DialogSimulator.FromDialog(main.Dialog, main.Options, new());
            HasMultipleChoices = _simulator.ActiveOutputs.Length > 1;
            HighlightRightPortrait = _simulator.ActiveNode.RightPortrait;
            LeftPortraitLabel = string.Empty;
            RightPortraitLabel = string.Empty;

            List<VmPathChoice> choices = new();

            for(int i = 0; i < 4; i++)
            {
                VmPathChoice choice;

                if(i >= _simulator.ActiveOutputs.Length)
                {
                    choice = new(i + 1, false, null);
                }
                else
                {
                    choice = new(i + 1, true, _simulator.ActiveOutputs[i].IconPath);
                }

                choices.Add(choice);
            }

            PathChoices = choices.ToArray();
            SelectedPathChoice = PathChoices[0];
        }


        private void UpdateView()
        {
            BeginChangeGroup();

            if(_simulator.EndReached)
            {
                TrackValueChange((v) => EndReached = v, EndReached, true);
            }
            else
            {
                TrackValueChange((v) => _selectedPathChoice = v, _selectedPathChoice, PathChoices[0]);
                TrackValueChange((v) => SelectedPathChoiceIndex = v, SelectedPathChoiceIndex, 0);
                TrackNotifyProperty(nameof(SelectedPathChoice));

                TrackValueChange((v) => HighlightRightPortrait = v, HighlightRightPortrait, _simulator.ActiveNode.RightPortrait);
                TrackValueChange((v) => HasMultipleChoices = v, HasMultipleChoices, _simulator.ActiveOutputs.Length > 1);

                SimulatorNodeOutput output = _simulator.ActiveOutputs[0];
                TrackValueChange((v) => Text = v, Text, output.Text);

                if (HighlightRightPortrait)
                {
                    TrackValueChange((v) => RightPortraitLabel = v, RightPortraitLabel, output.Label);
                    TrackValueChange((v) => RightPortraitPath = v, RightPortraitPath, output.PortraitPath);
                }
                else
                {
                    TrackValueChange((v) => LeftPortraitLabel = v, LeftPortraitLabel, output.Label);
                    TrackValueChange((v) => LeftPortraitPath = v, LeftPortraitPath, output.PortraitPath);
                }

                for (int i = 0; i < 4; i++)
                {
                    VmPathChoice choice = PathChoices[i];

                    if (i >= _simulator.ActiveOutputs.Length)
                    {
                        choice.Update(false, null);
                    }
                    else
                    {
                        choice.Update(true, _simulator.ActiveOutputs[i].IconPath);
                    }
                }
            }

            EndChangeGroup();
        }

        private void Reset()
        {
            BeginChangeGroup();
            _simulator.Reset();
            TrackValueChange((v) => EndReached = v, EndReached, false);

            if (!HighlightRightPortrait)
            {
                TrackValueChange((v) => RightPortraitLabel = v, RightPortraitLabel, string.Empty);
                TrackValueChange((v) => RightPortraitPath = v, RightPortraitPath, null);
            }
            else
            {
                TrackValueChange((v) => LeftPortraitLabel = v, LeftPortraitLabel, string.Empty);
                TrackValueChange((v) => LeftPortraitPath = v, LeftPortraitPath, null);
            }

            UpdateView();
            EndChangeGroup();
        }

        [MemberNotNull(nameof(Text))]
        private void SelectChoice(int index)
        {
            SelectedPathChoiceIndex = index;

            SimulatorNodeOutput output = _simulator.ActiveOutputs[index];

            Text = output.Text;

            if (HighlightRightPortrait)
            {
                RightPortraitLabel = output.Label;
                RightPortraitPath = output.PortraitPath;
            }
            else
            {
                LeftPortraitLabel = output.Label;
                LeftPortraitPath = output.PortraitPath;
            }
        }

        private void Next()
        {
            if (_simulator.EndReached)
                return;

            BeginChangeGroup();
            _simulator.Next(SelectedPathChoiceIndex);
            UpdateView();
            EndChangeGroup();
        }

        private void Undo()
            => UndoChange();

        private void Redo()
            => RedoChange();
    }
}
