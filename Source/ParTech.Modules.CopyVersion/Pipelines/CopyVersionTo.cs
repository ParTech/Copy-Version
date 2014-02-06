namespace ParTech.Modules.CopyVersion.Pipelines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using ParTech.Modules.CopyVersion.Utils;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Shell.Framework.Pipelines;
    using Sitecore.Web.UI.Sheer;

    /// <summary>
    /// Copy Version To content editor pipeline processor.
    /// </summary>
    public class CopyVersionTo : CopyItems
    {
        /// <summary>
        /// Executes the pipeline processor.
        /// </summary>
        /// <param name="args"></param>
        public override void Execute(CopyItemsArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            string language = args.Parameters["language"];
            
            Assert.ArgumentNotNull(language, "language");

            using (new LanguageSwitcher(language))
            {
                Item destination = CopyItems.GetDatabase(args)
                    .GetItem(args.Parameters["destination"]);

                Assert.IsNotNull(destination, args.Parameters["destination"]);

                List<Item> sourceItems = GetItems(args);

                if (sourceItems != null && sourceItems.Any())
                {
                    Item source = sourceItems.First();

                    CopyVersionUtil.CopyLatestVersion(source, destination);

                    args.Copies = new Item[] { destination };

                    SheerResponse.Alert("Version was successfully copied.");
                }
            }
        }
    }
}