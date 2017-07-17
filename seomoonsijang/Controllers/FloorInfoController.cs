using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace seomoonsijang.Controllers
{
    public class FloorInfoController : Controller
    {
        public ActionResult SecondBaseFirst()
        {
            ViewBag.Message = "2지구 지하1층 상가 위치 정보 입니다.";
            return View();
        }
        public ActionResult SecondFirst()
        {
            ViewBag.Message = "2지구 지상1층 상가 위치 정보 입니다.";
            return View();
        }
        public ActionResult SecondSecond()
        {
            ViewBag.Message = "2지구 지상2층 상가 위치 정보 입니다.";
            return View();
        }
        public ActionResult SecondThird()
        {
            ViewBag.Message = "2지구 지상3층 상가 위치 정보 입니다.";
            return View();
        }
        public ActionResult SecondForth()
        {
            ViewBag.Message = "2지구 지상4층 상가 위치 정보 입니다.";
            return View();
        }

        public ActionResult FifthFirst()
        {
            ViewBag.Message = "5지구 지상1층 상가 위치 정보 입니다.";
            return View();
        }
        public ActionResult FifthSecond()
        {
            ViewBag.Message = "5지구 지상2층 상가 위치 정보 입니다.";
            return View();
        }

        public ActionResult DongsanFirst()
        {
            ViewBag.Message = "동산상가 지상1층 상가 위치 정보 입니다.";
            return View();
        }
        public ActionResult DongsanSecond()
        {
            ViewBag.Message = "동산상가 지상2층 상가 위치 정보 입니다.";
            return View();
        }

        public ActionResult Union()
        {
            ViewBag.Message = "상가연합회 만남의 광장 정보 입니다.";
            return View();
        }
    }
}