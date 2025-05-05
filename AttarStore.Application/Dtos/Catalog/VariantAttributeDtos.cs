using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Application.Dtos.Catalog
{
    /// <summary>
    /// Incoming DTO to assign one option‐value to a new or updated variant.
    /// </summary>
    public class VariantAttributeDto
    {
        /// <summary>
        /// The VariantOption (e.g. “Size”) to assign.
        /// </summary>
        public int OptionId { get; set; }

        /// <summary>
        /// The chosen VariantOptionValue (e.g. “Large”) for that option.
        /// </summary>
        public int ValueId { get; set; }
    }

    /// <summary>
    /// Outgoing DTO when returning a variant’s attributes.
    /// </summary>
    public class VariantAttributeViewDto
    {
        /// <summary>
        /// The VariantOption’s ID (e.g. “Size”).
        /// </summary>
        public int OptionId { get; set; }

        /// <summary>
        /// The VariantOption’s name (e.g. “Size”).
        /// </summary>
        public string OptionName { get; set; }

        /// <summary>
        /// The specific VariantOptionValue’s ID (e.g. “Large”).
        /// </summary>
        public int ValueId { get; set; }

        /// <summary>
        /// The specific VariantOptionValue’s text (e.g. “Large”).
        /// </summary>
        public string Value { get; set; }
    }
}
