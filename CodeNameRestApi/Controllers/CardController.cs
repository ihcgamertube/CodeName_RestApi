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

        [HttpGet("/{word:maxlength(15)}", Name = "GetCard")]
        public ActionResult<string> GetCardIfExists(string word)
        {
            var handler = _cardService._mongoHandler;
            Task<CardDocument> cardTask = handler.FindCardAsync(word);
            cardTask.Wait();

            CardDocument card = cardTask.Result;
            if (card == null)
            {
                return NotFound();
            }

            return card.Word;
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
        public ActionResult<string> PostCardToDatabase(string word)
        {
            var handler = (WordsDatabaseAPI.DatabaseModels.MongoHandler)_cardService._mongoHandler;
            CardDocument card = new CardDocument(word);

            if (card == null)
                return NotFound();

            bool isSuccesfull = handler.InsertCard(card);
            if (!isSuccesfull)
                return BadRequest();

            return CreatedAtRoute("GetCard", new { word = card.Word }, card.Word);
        }

        [HttpPut("{word}/{newWord}")]
        public IActionResult UpdateCardInDatabase(string word, string newWord)
        {
            bool isSuccesfull = _cardService._mongoHandler.UpdateWord(word, newWord);
            if (!isSuccesfull)
                return BadRequest("Update Failed!");

            return NoContent();
        }
    }
}
