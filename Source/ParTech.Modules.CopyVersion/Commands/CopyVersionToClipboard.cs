namespace ParTech.Modules.CopyVersion.Commands
{
    using System;
    using System.Linq;
    using System.Text;
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Web.UI.Sheer;

    /// <summary>
    /// Copy version to clipboard command.
    /// </summary>
    public class CopyVersionToClipboard : ClipboardCommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="context"></param>
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            if (!ClipboardCommand.IsSupported(true) || context.Items.Length != 1 || context.Items.FirstOrDefault() == null)
            {
                return;
            }

            SheerResponse.Eval(string.Format("window.clipboardData.setData(\"Text\", \"sitecore:copyversion:{0}:{1}\")", context.Items.First().ID, context.Items.First().Language.Name));
        }

        /// <summary>
        /// Queries the state of the command for the specified context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            if (!ClipboardCommand.IsSupported(false))
            {
                return CommandState.Hidden;
            }

            if (context.Items.Length != 1 || !context.Items.First().Access.CanRead() || context.Items.First().Versions.Count == 0)
            {
                return CommandState.Disabled;
            }
           
            return base.QueryState(context);
        }
    }
}
