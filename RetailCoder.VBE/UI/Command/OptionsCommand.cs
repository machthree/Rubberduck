﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rubberduck.Settings;
using Rubberduck.UI.Settings;

namespace Rubberduck.UI.Command
{
    public class OptionsCommand : ICommand
    {
        private readonly IGeneralConfigService _service;
        public OptionsCommand(IGeneralConfigService service)
        {
            _service = service;
        }

        public void Execute()
        {
            using (var window = new SettingsDialog(_service))
            {
                window.ShowDialog();
            }
        }
    }

    public class OptionsCommandMenuItem : CommandMenuItemBase
    {
        public OptionsCommandMenuItem(ICommand command) : base(command)
        {
        }

        public override string Key { get { return "RubberduckMenu_Options"; } }
        public override bool BeginGroup { get { return true; } }
        public override int DisplayOrder { get { return (int)RubberduckMenuItemDisplayOrder.Options; } }
    }
}
