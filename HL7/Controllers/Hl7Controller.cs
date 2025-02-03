using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HL7.Services.Interfaces;
using HL7.Services;
using HL7.Helper;
namespace HL7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Hl7Controller : ControllerBase
    {
        private readonly IInsertHl7DataService _insertHl7DataService;
        public Hl7Controller(IInsertHl7DataService insertHl7DataService) 
        {
            _insertHl7DataService = insertHl7DataService;
        }
        [HttpPost("parse")]
        public IActionResult ParseAndStoreHL7(string hl7Message)
        {
            if (string.IsNullOrWhiteSpace(hl7Message))
                return BadRequest("HL7 message is empty");
            var parsedData = HL7ParserHelper.ParseHL7Message(hl7Message);
            int insertedId = _insertHl7DataService.SaveHl7JsonToDatabase(hl7Message);
            return Ok(new { Message = "HL7 Parsed and Stored", RecordId = insertedId, Data = parsedData });
        }
    }
}



