using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu(menuName = "CustomTile")]
public class WallCustomRuleTile : RuleTile<WallCustomRuleTile.Neighbor>
{
    public bool customField;
    public bool checkSelf;
    public TileBase[] tilesToConnect;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int SpecifiedPlus = 1;
        public const int NotSpecifiedPlus = 2;
        public const int This = 3;
        public const int NotThis = 4;
        public const int Any = 6;
        public const int Specified = 5;
        public const int Nothing = 7;

        //public const int Null = 3;
        //public const int NotNull = 4;
    }



    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.This: return This(tile);
            case Neighbor.NotThis: return NotThis(tile);
            case Neighbor.Any: return Any(tile);
            case Neighbor.Specified: return Specified(tile);
            case Neighbor.SpecifiedPlus: return SpecifiedPlus(tile);
            case Neighbor.NotSpecifiedPlus: return NotSpecifiedPlus(tile);
            case Neighbor.Nothing: return Nothing(tile);
                //case Neighbor.Null: return tile == null;
                //case Neighbor.NotNull: return tile != null;
        }
        return base.RuleMatch(neighbor, tile);
    }
    //Any tiles in list
    private bool This(TileBase tile)
    {
        return tile == this;
    }
    //Not this tile
    private bool NotThis(TileBase tile)
    {
        return tile != this;
    }
    //Not null
    private bool Any(TileBase tile)
    {
        if (checkSelf) return tile != null;
        else return tile != null && tile != this;
    }
    //Tile just in list
    private bool Specified(TileBase tile)
    {
        return tilesToConnect.Contains(tile);
    }

    private bool SpecifiedPlus(TileBase tile)
    {
        return tilesToConnect.Contains(tile) || tile == this;
    }

    private bool NotSpecifiedPlus(TileBase tile)
    {
        return !SpecifiedPlus(tile);
    }
    //No tile
    private bool Nothing(TileBase tile)
    {
        return tile == null;
    }
}