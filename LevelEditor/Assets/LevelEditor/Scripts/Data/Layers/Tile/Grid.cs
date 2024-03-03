using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Grid {
    public GridCell[,] Cells;
    public Integer2 Size;

    public Grid() { FindNeighbors(); }

    public Grid(int width, int height) {
        ReInit(width, height);
    }

    public void FindNeighbors() {
        if(Cells == null) return;

        for(int x = 0; x < Cells.GetLength(0); x++) {
            for(int y = 0; y < Cells.GetLength(1); y++) {
                if(x - 1 >= 0) GetCell(x, y).LEFT = GetCell(x - 1, y);
                if(y - 1 >= 0) GetCell(x, y).DOWN = GetCell(x, y - 1);
                if(x + 1 < Cells.GetLength(0)) GetCell(x, y).RIGHT = GetCell(x + 1, y);
                if(y + 1 < Cells.GetLength(1)) GetCell(x, y).UP = GetCell(x, y + 1);
            }
        }
    }

    public void ReInit(int width, int height, bool isNew = true) {
        GridSettingsModel model = ModuleContainer.I.ViewsController.GridSettingsView.GetModel() as GridSettingsModel;

        Size = new Integer2(width, height);

        int offsetX = 0;
        int offsetY = 0;

        if(Size.X % 2 == 1) offsetX = -model.GetFullWidth() / 2;
        if(Size.Y % 2 == 1) offsetY = -(model.GetHeight() - model.GetTrueHeight()) / 2;

        if(isNew) {
            Cells = new GridCell[Size.X, Size.Y];

            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    Cells[x, y] = new GridCell(string.Empty, new Integer2(((x - width / 2 + 1) * model.GetWidth() + (y - height / 2) * model.Point4.X) + offsetX, ((y - height / 2) * model.GetHeight() + (x - width / 2 + 1) * -model.Point2.Y) + offsetY), x, y);
                }
            }
        } else {
            GridCell[,] oldArray = Cells;
            Cells = new GridCell[Size.X, Size.Y];

            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    if(x < oldArray.GetLength(0) && y < oldArray.GetLength(1)) {
                        if(oldArray[x, y] != null && oldArray[x, y].SpritePath != null) {
                            Cells[x, y] = new GridCell(oldArray[x, y].SpritePath, new Integer2((x - width / 2 + 1) * model.GetWidth() + (y - height / 2) * model.Point4.X, ((y - height / 2) * model.GetHeight() + (x - width / 2 + 1) * -model.Point2.Y) + 0), x, y);
                            continue;
                        }
                    }
                    Cells[x, y] = new GridCell(string.Empty, new Integer2((x - width / 2 + 1) * model.GetWidth() + (y - height / 2) * model.Point4.X, ((y - height / 2) * model.GetHeight() + (x - width / 2 + 1) * -model.Point2.Y) + 0), x, y);
                }
            }
        }

        FindNeighbors();
    }

    public GridCell GetCell(int x, int y) {
        return Cells[x, y];
    }

    public void TileDraw(Integer2 point, string spritePath = "") {
        int widthGrid = Cells.GetLength(0);
        int heightGrid = Cells.GetLength(0);

        for(int x = 0; x < widthGrid; x++) {
            for(int y = 0; y < heightGrid; y++) {
                if(Cells[x, y].CanUse(point)) {
                    Cells[x, y].SetSprite(spritePath);
                }
            }
        }
    }

    public void PrefabDraw(Integer2 point, string spritePath = "") {
        int widthGrid = Cells.GetLength(0);
        int heightGrid = Cells.GetLength(0);

        for(int x = 0; x < widthGrid; x++) {
            for(int y = 0; y < heightGrid; y++) {
                if(Cells[x,y].CanUse(point)) {
                    Cells[x, y].SetSprite(spritePath, false);
                }
            }
        }
    }
}