namespace ParTech.Modules.CopyVersion.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Pipelines;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Shell.Framework.Pipelines;
    using Sitecore.Web.UI.Sheer;

    /// <summary>
    /// Copy Version To command
    /// </summary>
    public class CopyVersionTo : Command
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="context"></param>
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context.Items, "items");

            if (context.Items.Any())
            {
                // Start the uiCopyVersionTo pipeline.
                Start("uiCopyVersionTo", new CopyItemsArgs(), context.Items.First().Database, context.Items);
            }
        }

        /// <summary>
        /// Queries the state of the command for the specified context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override CommandState QueryState(CommandContext context)
        {
            Error.AssertObject(context, "context");

            if (!context.Items.Any())
            {
                return CommandState.Disabled;
            }

            Item item = context.Items.First();

            if (!item.Access.CanRead())
            {
                return CommandState.Disabled;
            }

            return base.QueryState(context);
        }

        /// <summary>
        /// Starts a UI pipeline.
        /// </summary>
        /// <param name="pipelineName"></param>
        /// <param name="args"></param>
        /// <param name="database"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private static Pipeline Start(string pipelineName, ClientPipelineArgs args, Database database, Item[] items)
        {
            Assert.ArgumentNotNullOrEmpty(pipelineName, "pipelineName");
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(database, "database");
            Assert.ArgumentNotNull(items, "items");

            args.Parameters = new NameValueCollection
            {
                { "database", database.Name },
                { "items", items.First().ID.ToString() },
                { "language", items.First().Language.Name }
            };

            return Sitecore.Context.ClientPage.Start(pipelineName, args);
        }
    }
}