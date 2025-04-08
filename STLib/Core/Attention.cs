using System;
using System.Text.Json.Serialization;

namespace STLib.Core
{
    public readonly struct Attention
    {
        public string AttentionText 
        { 
            get => m_attention; 
        }
        public AttentionType Type
        {
            get => m_type;
        }
        public DateTime Created
        {
            get => m_created;
        }

        private readonly string m_attention;
        private readonly AttentionType m_type;
        private readonly DateTime m_created;

        public Attention(string attention, AttentionType type)
        {
            if (string.IsNullOrWhiteSpace(attention))
            {
                throw new ArgumentNullException(nameof(attention));
            }

            if (attention.Length > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(attention), "Attention cannot exceed 100 characters.");
            }

            if (attention.Contains("NULL"))
            {
                throw new ArgumentException("Attention cannot contain \"NULL\".", nameof(attention));
            }

            m_attention = attention;
            m_type = type;
            m_created = DateTime.UtcNow;
        }

        [JsonConstructor]
#pragma warning disable IDE0051
        private Attention(string attention, AttentionType type, DateTime created)
#pragma warning restore IDE0051
        {
            if (string.IsNullOrWhiteSpace(attention))
            {
                throw new ArgumentNullException(nameof(attention));
            }

            if (attention.Length > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(attention), "Attention cannot exceed 100 characters.");
            }

            if (attention.Contains("NULL"))
            {
                throw new ArgumentException("Attention cannot contain \"NULL\".", nameof(attention));
            }

            if (created.Equals(DateTime.MinValue) || created.Equals(DateTime.MaxValue))
            {
                throw new ArgumentOutOfRangeException(nameof(created), "Created date is invalid.");
            }

            m_attention = attention;
            m_type = type;
            m_created = created;
        }

        public override string ToString() => $"{m_created} [{m_type}] {m_attention}";
    }
}