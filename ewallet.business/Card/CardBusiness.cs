using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ewallet.repository.Card;
using ewallet.shared.Models;

namespace ewallet.business.Card
{
    public class CardBusiness : ICardBusiness
    {
        ICardRepository _repo;
        public CardBusiness()
        {
            _repo = new CardRepository();
        }
        public List<CardCommon> GetCardList(string UserId)
        {
            return _repo.GetCardList(UserId);
        }
        public Dictionary<string, string> GetCardType()
        {
            return _repo.GetCardType();
        }
        public CommonDbResponse InsertCard(CardCommon cardCommon)
        {
            return _repo.InsertCard(cardCommon);
        }

        public CommonDbResponse UpdateCard(CardCommon cardCommon)
        {
            return _repo.UpdateCard(cardCommon);
        }
        public CommonDbResponse EnableDisableCard(CardCommon cardCommon)
        {
            return _repo.EnableDisableCard(cardCommon);
        }

        public CommonDbResponse RequestCard(CardCommon cardCommon)
        {
            return _repo.RequestCard(cardCommon);
        }
        public List<CardCommon> GetApprovalList()
        {
            return _repo.GetApprovalList();
        }

        public CommonDbResponse CardApproval(CardCommon cardCommon)
        {
            return _repo.CardApproval(cardCommon);
        }

        public CommonDbResponse CardBalance(CardCommon cardCommon)
        {
            return _repo.CardBalance(cardCommon);
        }

        public CommonDbResponse CardUser(CardCommon cardCommon)
        {
            return _repo.CardUser(cardCommon);
        }
    }
}
