using InternetBanking.Database.Entities;
using System.Collections.Generic;

namespace InternetBanking.Database.Comparators
{
    public sealed class MessageThreadComparator : IEqualityComparer<MessageThread>
    {
        bool IEqualityComparer<MessageThread>.Equals(MessageThread x, MessageThread y)
        {
            return x.Id.Equals(y.Id);
        }

        int IEqualityComparer<MessageThread>.GetHashCode(MessageThread obj)
        {
            return obj is null ? 0 : obj.Id.GetHashCode();
        }
    }
}
