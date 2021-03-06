﻿using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using DataService.service.dao;
using Domain.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using System.Reflection;

namespace Domain.control
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class DepartmentControl : IHttpHandler
    {
        public HttpContext context;
        public void ProcessRequest(HttpContext context)
        {
            this.context = context;
            context.Response.ContentType = "text/plain";
            string method = context.Request.Form.Get("method");
            if (method == null)
            {
                method = context.Request.QueryString["method"];
            }
            switch (method)
            {
                case "getDepartmentTree":
                    getDepartmentTree();
                    break;
                case "getDepartment":
                    getDepartment();
                    break;
                case "saveDepartment":
                    saveDepartment();
                    break;
                case "deleteDepartment":
                    deleteDepartment();
                    break;
                case "getFacultyCombo" :
                    getFacultyCombo();
                    break;
                case "getProfessionCombo":
                    getProfessionCombo();
                    break;
                case "getClassGradeCombo":
                    getClassGradeCombo();
                    break;
                default:
                    context.Response.Write("-1");
                    break;
            }

        }

        private void getDepartment()
        {
            string type = context.Request.Form.Get("type");
            if (type == null)
                return;
            string id = context.Request.Form.Get("Id");
            String json = "";
            DepartmentService ds = new DepartmentService();
            switch (type)
            {
                case "学校":
                    School school = (School)ds.get(typeof(School), id);
                    json = JsonConvert.SerializeObject(school);
                    break;
                case "院系":
                    Faculty faculty = (Faculty)ds.get(typeof(Faculty), id);
                    json = JsonConvert.SerializeObject(faculty);
                    break;
                case "专业":
                    Profession profession = (Profession)ds.get(typeof(Profession), id);
                    json = JsonConvert.SerializeObject(profession);
                    break;
                case "班级":
                    ClassGrade classGrade = (ClassGrade)ds.get(typeof(ClassGrade), id);
                    json = JsonConvert.SerializeObject(classGrade);
                    break;
            }
            context.Response.Write(json);
        }

        private void deleteDepartment()
        {
            try
            {
                string type = context.Request.Form.Get("type");
                if (type == null)
                    return;
                string id = context.Request.Form.Get("Id");
                DepartmentService ds = new DepartmentService();
                switch (type)
                {
                    case "学校":
                        School school = (School)ds.get(typeof(School), id);
                        ds.del(school);
                        break;
                    case "院系":
                        Faculty faculty = (Faculty)ds.get(typeof(Faculty), id);
                        ds.del(faculty);
                        break;
                    case "专业":
                        Profession profession = (Profession)ds.get(typeof(Profession), id);
                        ds.del(profession);
                        break;
                    case "班级":
                        ClassGrade classGrade = (ClassGrade)ds.get(typeof(ClassGrade), id);
                        ds.del(classGrade);
                        break;
                }
               context.Response.Write("1");
            }
            catch (Exception e) {
                context.Response.Write("0");
            }
            
        }

        private void saveDepartment()
        {
            try
            {
                string type = context.Request.Form.Get("type");
                if (type == null)
                    return;
                string pId = context.Request.Form.Get("pId");
                DepartmentService ds = new DepartmentService();
                switch (type)
                {
                    case "学校":
                        School school = (School)colectionParameter(typeof(School));
                        ds.save(school);
                        break;
                    case "院系":
                        School s = (School)ds.get(typeof(School), pId);
                        Faculty faculty = (Faculty)colectionParameter(typeof(Faculty));
                        faculty.School = s;
                        ds.save(faculty);
                        break;
                    case "专业":
                        Faculty f = (Faculty)ds.get(typeof(Faculty), pId);
                        Profession profession = (Profession)colectionParameter(typeof(Profession));
                        profession.Faculty = f;
                        ds.save(profession);
                        break;
                    case "班级":
                        Profession p = (Profession)ds.get(typeof(Profession), pId);
                        ClassGrade classGrade = (ClassGrade)colectionParameter(typeof(ClassGrade));
                        classGrade.Profession = p;
                        ds.save(classGrade);
                        break;
                }
                context.Response.Write("1");
            }
            catch (Exception e) {
                context.Response.Write("0");
            }
            
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void getDepartmentTree()
        {
            DepartmentService ds = new DepartmentService();
            IList<EasyuiTree> list = ds.getDepartmentTree();
            String json = JsonConvert.SerializeObject(list);
            context.Response.Write(json);
        }

        public void addSchool() {
            try
            {
                School s = new School();
                DepartmentService ds = new DepartmentService();
                ds.save(s);
                context.Response.Write("1");
            }
            catch (Exception e) {
                context.Response.Write("0");
            }
            
        }

        public Object colectionParameter(Type type)
        {
            Object o = Activator.CreateInstance(type);
            setValue(o, context);

            HttpPostedFile hpf = context.Request.Files["DescriptImage"];
            if (hpf != null && hpf.FileName.Split('.').Length == 2)
            {
                string serverPath = "/uploadFile/headImg/" + System.DateTime.Now.Ticks + "." + hpf.FileName.Split('.')[1];
                string savePath = context.Server.MapPath(serverPath);//路径,相对于服务器当前的路径
                hpf.SaveAs(savePath);//保存
                PropertyInfo property = o.GetType().GetProperty("DescriptImage");
                property.SetValue(o, serverPath, null);
            }
            return o;
        }

        public Object setValue(Object o, HttpContext context)
        {
            string[] keys = context.Request.Form.AllKeys;
            foreach (string s in keys)
            {
                try
                {
                    PropertyInfo property = o.GetType().GetProperty(s);
                    if (property == null)
                    {
                        continue;
                    }
                    if (property.PropertyType == typeof(DateTime))
                    {
                        property.SetValue(o, Convert.ToDateTime(context.Request.Form.Get(s)), null);
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        property.SetValue(o, context.Request.Form.Get(s), null);
                    }
                    else if (property.PropertyType == typeof(int)) {
                        property.SetValue(o, Convert.ToInt32(context.Request.Form.Get(s)), null);
                    }

                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }

            }
            return o;
        }

        public void getFacultyCombo() {
            DepartmentService ds = new DepartmentService();
            IList<Faculty> facultyList = ds.getFacultyList();
            String json = JsonConvert.SerializeObject(facultyList);
            context.Response.Write(json);
        }

        public void getProfessionCombo(){
            string facultyID = context.Request.QueryString["facultyID"];
            DepartmentService ds = new DepartmentService();
            ISet<Profession> professionList = ds.getProfessionSet(facultyID);
            if (professionList.Count > 0)
            {
                String json = JsonConvert.SerializeObject(professionList);
                context.Response.Write(json);
            }
            
        }

        public void getClassGradeCombo(){
            string professionID = context.Request.QueryString["professionID"];
            DepartmentService ds = new DepartmentService();
            ISet<ClassGrade> classGradeList = ds.getClassGradeSet(professionID);
            if (classGradeList.Count > 0)
            {
                String json = JsonConvert.SerializeObject(classGradeList);
                context.Response.Write(json);
            }
            
        }
    }
}
