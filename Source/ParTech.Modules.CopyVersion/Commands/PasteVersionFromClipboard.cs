namespace ParTech.Modules.CopyVersion.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using ParTech.Modules.CopyVersion.Utils;
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Web.UI.Sheer;

    /// <summary>
    /// Paste version from clipboard command.
    /// </summary>
    public class PasteVersionFromClipboard : ClipboardCommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="context"></param>
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            if (!ClipboardCommand.IsSupported(true) || context.Items.Length != 1)
            {
                return;
            }

            Item item = context.Items.First();

            var parameters = new NameValueCollection();
            parameters["id"] = item.ID.ToString();
            parameters["database"] = item.Database.Name;
            parameters["fetched"] = "0";

            Context.ClientPage.Start(this, "Run", parameters);
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

            if (context.Items.Length != 1 || !context.Items.First().Access.CanCreate())
            {
                return CommandState.Disabled;
            }
            
            return base.QueryState(context);
        }

        /// <summary>
        /// Run the pipeline processor.
        /// </summary>
        /// <param name="args"></param>
        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.Parameters["fetched"] == "0")
            {
                // Fetch the data from the clipboard.
                Context.ClientPage.ClientResponse.Eval("window.clipboardData.getData(\"Text\")").Attributes["response"] = "1";

                args.Parameters["fetched"] = "1";
                args.WaitForPostBack();
            }
            else
            {
                string dbname = args.Parameters["database"];
                Database database = Factory.GetDatabase(dbname);

                Assert.IsNotNull(database, dbname);

                if (string.IsNullOrEmpty(args.Result))
                {
                    return;
                }

                if (!args.Result.StartsWith("sitecore:copyversion:"))
                {
                    SheerResponse.Alert("The data on the clipboard is not valid.\n\nTry copying the data again.");
                }
                else
                {
                    string[] parts = args.Result.Split(':');

                    if (parts.Length != 4 || !ID.IsID(parts[2]))
                    {
                        SheerResponse.Alert("The data on the clipboard is not valid.\n\nTry copying the data again.");
                        args.AbortPipeline();
                        return;
                    }

                    string id = parts[2];
                    string language = parts[3];

                    using (new LanguageSwitcher(language))
                    {
                        Item source = database.GetItem(id);
                        Item destination = database.GetItem(args.Parameters["id"]);

                        // Check if it's allowed to paste the version.
                        if (!this.ValidatePaste(source, destination))
                        {
                            args.AbortPipeline();
                            return;
                        }
                        
                        // Copy the last version from the source item to the destination item.
                        if (CopyVersionUtil.CopyLatestVersion(source, destination))
                        {
                            SheerResponse.Alert("Version was successfully copied.");
                            return;
                        }

                        SheerResponse.Alert("An error has occurred while pasting the item version");
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether the paste action is valid and alerts error messages to SheerResponse.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns>true if paste is valid, false if invalid.</returns>
        private bool ValidatePaste(Item source, Item destination)
        {
            if (source == null)
            {
                SheerResponse.Alert("The item that you want to paste could not be found.\n\nIt may have been deleted by another user.");
                return false;
            }

            if (destination == null)
            {
                SheerResponse.Alert("The destination item could not be found.\n\nIt may have been deleted by another user.");
                return false;
            }

            if (source.ID.Equals(destination.ID))
            {
                SheerResponse.Alert("You cannot paste an item version on the source item itself.");
                return false;
            }

            if (!source.TemplateID.Equals(destination.TemplateID))
            {
                SheerResponse.Alert("The destination item's template must match the source item's template.");
                return false;
            }

            return true;
        }
    }
}
