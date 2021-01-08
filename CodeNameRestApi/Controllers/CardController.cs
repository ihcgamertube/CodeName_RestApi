using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeNameRestApi.Services;
using Microsoft.AspNetCore.Mvc;
using WordsDatabaseAPI.DatabaseModels.CollectionModels;

namespace CodeNameRestApi.Controllers
{
    [Route("api/[controller]/english")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly CardService _cardService;

        public CardController(CardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet("/{id:maxlength(8)}", Name = "GetCard")]
        public ActionResult<CardDocument> GetCardAtIndex(string id)
        {
            var handler = _cardService._mongoHandler;
            Task<CardDocument> cardTask = handler.FindCardAtIndexAsync(uint.Parse(id));
            cardTask.Wait();

            CardDocument card = cardTask.Result;
            if (card == null)
            {
                return NotFound();
            }

            return card;
        }

        [HttpGet("/count/", Name = "GetCount")]
        public ActionResult<long> GetCardsCount()
        {
            Task<long> countTask = _cardService._mongoHandler.GetDocumentsCountAsync();
            countTask.Wait();
            return countTask.Result;
        }

        [HttpGet("/random/{numberOfRandomNumbers:int}")]
        public ActionResult<CardDocument[]> GetRandomCards(int numberOfRandomNumbers)
        {
            if (numberOfRandomNumbers <= 0)
                return BadRequest();

            if (GetCardsCount().Value < numberOfRandomNumbers)
                return BadRequest();
            
            Task<CardDocument[]> randomTask = _cardService._mongoHandler.FindMultipleRandomCardsAsync((uint)numberOfRandomNumbers);
            randomTask.Wait();

            CardDocument[] randomCards = randomTask.Result;
            if (randomCards.Length != numberOfRandomNumbers)
                return NotFound();

            return randomCards;
        }

        [HttpPost("")]
        public ActionResult<CardDocument> PostCardToDatabase(string word)
        {
            var handler = (WordsDatabaseAPI.DatabaseModels.MongoHandler)_cardService._mongoHandler;
            Task<CardDocument> cardTask = CardDocument.CreateBasedOnWordAsync(handler, word);
            cardTask.Wait();

            CardDocument card = cardTask.Result;

            if (card == null)
                return NotFound();

            bool isSuccesfull = handler.InsertCard(card);
            if (!isSuccesfull)
                return BadRequest();

            return CreatedAtRoute("GetCard", new { id = card.Id.ToString() }, card);
        }

        [HttpPut("{word}/{newWord}")]
        public IActionResult UpdateCardInDatabase(string word, string newWord)
        {
            _cardService._mongoHandler.RemoveWordAsync(word).Wait();

            var handler = (WordsDatabaseAPI.DatabaseModels.MongoHandler)_cardService._mongoHandler;
            Task<CardDocument> cardTask = CardDocument.CreateBasedOnWordAsync(handler, newWord);
            cardTask.Wait();

            bool isSuccesfull = handler.InsertCard(cardTask.Result);
            if (!isSuccesfull)
                return BadRequest();

            return NoContent();
        }

        [HttpDelete("{word}")]
        public IActionResult DeleteWord(string word)
        {
            Task<bool> removeTask = _cardService._mongoHandler.RemoveWordAsync(word);
            removeTask.Wait();

            return (removeTask.Result) ? NoContent() : NotFound(); 
        }

    }
}
