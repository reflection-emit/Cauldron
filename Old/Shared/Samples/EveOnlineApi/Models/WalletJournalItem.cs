using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace EveOnlineApi.Models
{
    [DebuggerDisplay("{Amount}")]
    public sealed class WalletJournalItem
    {
        /// <summary>
        /// Transaction amount. Positive when value transferred to the first owner. Negative otherwise.
        /// </summary>
        [XmlDeserializerAttribute("amount")]
        [JsonProperty("amount")]
        public double Amount { get; private set; }

        /// <summary>
        /// Ref type dependent argument value. See the Reference Type table.
        /// </summary>
        [XmlDeserializerAttribute("argName1")]
        [JsonProperty("argName1")]
        public int ArgumentId1 { get; private set; }

        /// <summary>
        /// Ref type dependent argument name. See the Reference Type table.
        /// </summary>
        [XmlDeserializerAttribute("itemID")]
        [JsonProperty("itemID")]
        public string ArgumentName1 { get; private set; }

        /// <summary>
        /// Wallet balance after transaction occurred.
        /// </summary>
        [XmlDeserializerAttribute("balance")]
        [JsonProperty("balance")]
        public double Balance { get; private set; }

        /// <summary>
        /// Date and time of transaction.
        /// </summary>
        [XmlDeserializerAttribute("date")]
        [JsonProperty("date")]
        public DateTime Date { get; private set; }

        public bool IsEarning { get { return this.Amount > 0; } }

        /// <summary>
        /// Character or corporation ID of first party. For NPC corporations, see the appropriate cross reference.
        /// </summary>
        [XmlDeserializerAttribute("ownerID1")]
        [JsonProperty("ownerID1")]
        public long OwnerId1 { get; private set; }

        /// <summary>
        /// Character or corporation ID of second party. For NPC corporations, see the appropriate cross reference.
        /// </summary>
        [XmlDeserializerAttribute("ownerID2")]
        [JsonProperty("ownerID2")]
        public long OwnerId2 { get; private set; }

        /// <summary>
        /// Name of first party in transaction.
        /// </summary>
        [XmlDeserializerAttribute("ownerName1")]
        [JsonProperty("ownerName1")]
        public string OwnerName1 { get; private set; }

        /// <summary>
        /// Name of second party in transaction.
        /// </summary>
        [XmlDeserializerAttribute("ownerName2")]
        [JsonProperty("ownerName2")]
        public string OwnerName2 { get; private set; }

        /// <summary>
        /// Ref type dependent reason. See the Reference Type table.
        /// </summary>
        [XmlDeserializerAttribute("reason")]
        [JsonProperty("reason")]
        public string Reason { get; private set; }

        /// <summary>
        /// Unique journal reference ID.
        /// </summary>
        [XmlDeserializerAttribute("refID")]
        [JsonProperty("refID")]
        public long ReferenceId { get; private set; }

        /// <summary>
        /// Transaction type.
        /// </summary>
        [XmlDeserializerAttribute("refTypeID")]
        [JsonProperty("refTypeID")]
        public int ReferenceTypeId { get; private set; }

        /// <summary>
        /// Tax amount received for tax related transactions.
        /// </summary>
        [XmlDeserializerAttribute("taxAmount")]
        [JsonProperty("taxAmount")]
        public double TaxAmount { get; private set; }

        /// <summary>
        /// For tax related transactions, gives the corporation ID of the entity receiving the tax.
        /// </summary>
        [XmlDeserializerAttribute("taxReceiverID")]
        [JsonProperty("taxReceiverID")]
        public long TaxRecieverId { get; private set; }
    }
}