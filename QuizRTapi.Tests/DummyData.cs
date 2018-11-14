using System;
using Xunit;
using Moq;
using System.Linq;
using System.Collections.Generic;
using QuizRT.Models;
using Microsoft.AspNetCore.Mvc;
using QuizRTapi.Controllers;
namespace GingerNote.Tests
{
    public class DummyData
    {
        // public List<GingerNoteC> DummyMock(){
        //     return new List<GingerNoteC>{
        //         new GingerNoteC{
        //             NoteId = 1,
        //             NoteTitle = "WishList",
        //             NoteBody = "Nothing as such",
        //             NoteChecklist = new List<Checklist>{
        //                 new Checklist{
        //                     ChecklistId = 1,
        //                     ChecklistName = "Laptop",
        //                     NoteId = 1
        //                 }, new Checklist{
        //                     ChecklistId = 2,
        //                     ChecklistName = "Bike",
        //                     NoteId = 1
        //                 }
        //             },
        //             NoteLabel = new List<Label>{
        //                 new Label{
        //                     LabelId = 1,
        //                     LabelName = "Casual",
        //                     NoteId = 1
        //                 }
        //             }
        //         },
        //         new GingerNoteC{
        //             NoteId = 2,
        //             NoteTitle = "Courses",
        //             NoteChecklist = new List<Checklist>{
        //                 new Checklist{
        //                     ChecklistId = 3,
        //                     ChecklistName = "Bootstrap",
        //                     NoteId = 2
        //                 }
        //             },
        //             NoteLabel = new List<Label>{
        //                 new Label{
        //                     LabelId = 2,
        //                     LabelName = ".Net",
        //                     NoteId = 2
        //                 }, new Label{
        //                     LabelId = 3,
        //                     LabelName = "Casual",
        //                     NoteId = 2
        //                 }
        //             }
        //         }
        //     };
        // }
    }
}
