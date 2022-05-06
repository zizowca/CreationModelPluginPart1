﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace CreationModelPlugin
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand

    {
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            List<Level> listLevel = new FilteredElementCollector(doc)
             .OfClass(typeof(Level))
             .OfType<Level>()
             .ToList();

            Level level1 = listLevel
                      .Where(x => x.Name.Equals("Уровень 1"))
                      .FirstOrDefault();

            Level level2 = listLevel
                  .Where(x => x.Name.Equals("Уровень 2"))
                  .FirstOrDefault();
            CreateWall(doc, level1, level2);
            
            //AddDoor(doc, level1, walls[0]);
            
           
            return Result.Succeeded;
        }
        private void CreateWall(Document doc, Level level1, Level level2 )
        {
            double width = UnitUtils.ConvertToInternalUnits(10000, UnitTypeId.Millimeters);
            double depth = UnitUtils.ConvertToInternalUnits(5000, UnitTypeId.Millimeters);
            double dx = width / 2;
            double dy = depth / 2;

            List<XYZ> points = new List<XYZ>();
            points.Add(new XYZ(-dx, -dy, 0));
            points.Add(new XYZ(dx, -dy, 0));
            points.Add(new XYZ(dx, dy, 0));
            points.Add(new XYZ(-dx, dy, 0));
            points.Add(new XYZ(-dx, -dy, 0));
            List<Wall> walls = new List<Wall>();

            Transaction transaction = new Transaction(doc, "create walls");
            transaction.Start();
            for (int i = 0; i < 4; i++)
            {
                Line line = Line.CreateBound(points[i], points[i + 1]);
                Wall wall = Wall.Create(doc, line, level1.Id, false);
                walls.Add(wall);
                wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level2.Id);
            }
            transaction.Commit();
        }

        //private void AddDoor (Document doc, Level level1, Wall wall)
        //{
        //    FamilySymbol doorType = new FilteredElementCollector(doc)
        //        .OfClass(typeof(FamilySymbol))
        //        .OfCategory(BuiltInCategory.OST_Doors)
        //        .OfType<FamilySymbol>()
        //        .Where(x => x.Name.Equals("0915 x 2134 мм"))
        //        .Where(x => x.FamilyName.Equals("Одиночные-Щитовые"))
        //        .FirstOrDefault();
        //   LocationCurve hostCurve= wall.Location as LocationCurve;
        //    XYZ point1 = hostCurve.Curve.GetEndPoint(0);
        //    XYZ point2 = hostCurve.Curve.GetEndPoint(1);
        //    XYZ point = (point1 + point2) / 2;
           
        //    if(!doorType.IsActive)
        //        doorType.Activate();

        //    doc.Create.NewFamilyInstance(point, doorType, wall, level1, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
        //}
                
    }
}
