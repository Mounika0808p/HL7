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
        private readonly InsertHl7DataService _hl7Repository;
        private readonly IConfiguration _configuartion;
        public Hl7Controller(IConfiguration configuration,InsertHl7DataService insertHl7DataService) 
        { 
            //_hl7Repository = new InsertHl7DataService(configuration); 
            insertHl7DataService = _hl7Repository;
            this._configuartion = configuration;
        }
        [HttpPost("parse")]
        public IActionResult ParseAndStoreHL7([FromBody] string hl7Message)
        {
            if (string.IsNullOrWhiteSpace(hl7Message))
                return BadRequest("HL7 message is empty");
            var parsedData = HL7ParserHelper.ParseHL7Message(hl7Message);
            int insertedId = _hl7Repository.InsertHl7Data(parsedData);
            return Ok(new { Message = "HL7 Parsed and Stored", RecordId = insertedId, Data = parsedData });
        }
    }
}
