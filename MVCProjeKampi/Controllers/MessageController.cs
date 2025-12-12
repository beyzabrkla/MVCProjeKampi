using Antlr.Runtime.Misc;
using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using BusinessLayer.ValidationRules;
using DataAccessLayer.Abstract;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using FluentValidation.Results;
using Ganss.Xss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCProjeKampi.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageservice;
        MessageValidator messagevalidator = new MessageValidator();

        public MessageController(IMessageService messageservice, MessageValidator messagevalidator)
        {
            _messageservice = messageservice;
            this.messagevalidator = messagevalidator;
        }

        public ActionResult Inbox()
        {
            var messagelist = _messageservice.GetListInbox();
            return View(messagelist);
        }

        public ActionResult SendBox()
        {
            var messagelist = _messageservice.GetListSendbox();
            return View(messagelist);
        }

        public ActionResult GetInboxMessageDetails(int id)
        {
            var values = _messageservice.GetById(id);
            return View(values);
        }

        public ActionResult GetSendboxMessageDetails(int id)
        {
            var values = _messageservice.GetById(id);
            return View(values); 
        }

        [HttpGet]
        public ActionResult NewMessage(int? id)
        {
            if (id.HasValue)
            {
                var draft = _messageservice.GetById(id.Value);
                return View(draft); // Taslak içeriğini formda gösterecek
            }
            return View(); 
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult NewMessage(Message p, string action)
        {
            p.SenderMail = "admin@gmail.com";
            p.MessageDate = DateTime.Now;
            var sanitizer = new HtmlSanitizer(); //mesaj içeriğindeki zararlı yazılımlar için 
            p.MessageContent = sanitizer.Sanitize(p.MessageContent);

            if (action == "send")
            {
                // GÖNDERME İŞLEMİ: Fluent Validation kontrolü yapılır.
                ValidationResult results = messagevalidator.Validate(p);

                if (results.IsValid)
                {
                    p.IsDraft = false;
                    _messageservice.MessageAdd(p); 
                    return RedirectToAction("SendBox");
                }
                else
                {
                    foreach (var x in results.Errors)
                    {
                        ModelState.AddModelError(x.PropertyName, x.ErrorMessage);
                    }
                    return View(p);
                }
            }
            else if (action == "save")
            {
                p.IsDraft = true; 
                _messageservice.MessageAdd(p); 
                return RedirectToAction("Drafts");
            }
            return View(p);
        }

        public ActionResult Drafts()
        {
            var draftMessages = _messageservice.GetDraftMessages(); 
            return View(draftMessages);
        }

    }
}