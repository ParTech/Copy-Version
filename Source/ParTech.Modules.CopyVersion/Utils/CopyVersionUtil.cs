namespace ParTech.Modules.CopyVersion.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;

    /// <summary>
    /// Utility methods for version copying.
    /// </summary>
    public static class CopyVersionUtil
    {
        /// <summary>
        /// Copy the latest version of the source item to the destination item.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns>true if successful, false if an exception occurred.</returns>
        public static bool CopyLatestVersion(Item source, Item destination)
        {
            try
            {
                source.Fields.ReadAll();

                destination = destination.Versions.AddVersion();
                destination.Editing.BeginEdit();

                foreach (Field field in source.Fields)
                {
                    destination.Fields[field.ID].SetValue(field.Value, true);
                }

                destination.Editing.EndEdit();
                destination.Editing.AcceptChanges();
            }
            catch (Exception ex)
            {
                destination.Editing.CancelEdit();
                return false;
            }

            return true;
        }
    }
}