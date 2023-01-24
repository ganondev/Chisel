using Godot;
using System;
using System.Collections.Specialized;
using System.Linq;using System.Security.Cryptography;
using Chisel;
using Godot.Collections;
using NumSharp;
using Array = Godot.Collections.Array;
using ManagedDict = System.Collections.Generic.Dictionary<Godot.Vector2, (int, HintGrouping)>;

enum HintGrouping : int
{
	One,
	Two,
	ThreePlus,
}

public class PuzzleSolver : StaticBody
{
	private const int TextureSheetWidth = 10;
	private const float TextureTileSize = (float)(1.0 / TextureSheetWidth);
		
	private NDArray _data;
	private readonly ManagedDict xHints = 
		new ManagedDict {
			{new Vector2(0, 0) , (1, HintGrouping.One)},
			{new Vector2(1, 0) , (4, HintGrouping.Two)},
			{new Vector2(2, 0) , (4, HintGrouping.Two)},
			{new Vector2(3, 0) , (2, HintGrouping.Two)},
			{new Vector2(5, 0) , (2, HintGrouping.Two)},
			{new Vector2(6, 0) , (1, HintGrouping.One)},
			{new Vector2(7, 0) , (3, HintGrouping.ThreePlus)},
			{new Vector2(8, 0) , (5, HintGrouping.Two)},
			{new Vector2(9, 0) , (1, HintGrouping.One)},
			
			{new Vector2(1, 1) , (7, HintGrouping.One)},
			{new Vector2(2, 1) , (4, HintGrouping.Two)},
			{new Vector2(3, 1) , (3, HintGrouping.Two)},
			{new Vector2(6, 1) , (2, HintGrouping.One)},
			{new Vector2(7, 1) , (0, HintGrouping.One)},
			{new Vector2(8, 1) , (2, HintGrouping.One)},
			{new Vector2(9, 1) , (0, HintGrouping.One)},
			
			{new Vector2(0, 2) , (6, HintGrouping.One)},
			{new Vector2(1, 2) , (9, HintGrouping.Two)},
			{new Vector2(4, 2) , (4, HintGrouping.One)},
			{new Vector2(8, 2) , (1, HintGrouping.One)},
			
			{new Vector2(0, 3) , (4, HintGrouping.One)},
			{new Vector2(6, 3) , (4, HintGrouping.One)},
			{new Vector2(7, 3) , (4, HintGrouping.One)},
			{new Vector2(8, 3) , (3, HintGrouping.One)},

			{new Vector2(1, 4) , (2, HintGrouping.Two)},
			{new Vector2(2, 4) , (1, HintGrouping.One)},
			{new Vector2(3, 4) , (3, HintGrouping.One)},
			{new Vector2(4, 4) , (3, HintGrouping.One)},
			{new Vector2(5, 4) , (6, HintGrouping.Two)},
			{new Vector2(7, 4) , (6, HintGrouping.Two)},
			{new Vector2(8, 4) , (2, HintGrouping.One)},
			{new Vector2(9, 4) , (0, HintGrouping.One)},
			
			{new Vector2(1, 5) , (0, HintGrouping.One)},
			{new Vector2(2, 5) , (0, HintGrouping.One)},
			{new Vector2(3, 5) , (0, HintGrouping.One)},
			{new Vector2(5, 5) , (3, HintGrouping.Two)},
			{new Vector2(6, 5) , (7, HintGrouping.Two)},
			{new Vector2(7, 5) , (4, HintGrouping.Two)},
			{new Vector2(9, 5) , (1, HintGrouping.One)},
			
			{new Vector2(0, 6) , (1, HintGrouping.One)},
			{new Vector2(1, 6) , (1, HintGrouping.One)},
			{new Vector2(2, 6) , (1, HintGrouping.One)},
			{new Vector2(3, 6) , (1, HintGrouping.One)},
			{new Vector2(4, 6) , (1, HintGrouping.One)},
			{new Vector2(6, 6) , (7, HintGrouping.One)},
			{new Vector2(7, 6) , (7, HintGrouping.ThreePlus)},
			{new Vector2(8, 6) , (1, HintGrouping.One)},
			{new Vector2(9, 6) , (2, HintGrouping.One)},
			
			{new Vector2(2, 7) , (1, HintGrouping.One)},
			{new Vector2(4, 7) , (2, HintGrouping.Two)},
			{new Vector2(5, 7) , (6, HintGrouping.One)},
			{new Vector2(7, 7) , (7, HintGrouping.Two)},
			{new Vector2(8, 7) , (2, HintGrouping.Two)},
			{new Vector2(9, 7) , (2, HintGrouping.Two)},
			
			{new Vector2(0, 8) , (0, HintGrouping.One)},
			{new Vector2(1, 8) , (1, HintGrouping.One)},
			{new Vector2(2, 8) , (1, HintGrouping.One)},
			{new Vector2(3, 8) , (3, HintGrouping.Two)},
			{new Vector2(4, 8) , (0, HintGrouping.One)},
			{new Vector2(6, 8) , (4, HintGrouping.One)},
			{new Vector2(7, 8) , (3, HintGrouping.Two)},
			{new Vector2(8, 8) , (1, HintGrouping.One)},
			{new Vector2(9, 8) , (3, HintGrouping.Two)},

			{new Vector2(1, 9) , (0, HintGrouping.One)},
			{new Vector2(2, 9) , (1, HintGrouping.One)},
			{new Vector2(4, 9) , (0, HintGrouping.One)},
			{new Vector2(6, 9) , (0, HintGrouping.One)},
			{new Vector2(8, 9) , (1, HintGrouping.One)},
			{new Vector2(9, 9) , (0, HintGrouping.One)},
			
		};
	private readonly ManagedDict yHints =
		new ManagedDict {
			
			{new Vector2(1, 9) , (0, HintGrouping.One)},
			{new Vector2(4, 9) , (1, HintGrouping.One)},
			{new Vector2(5, 9) , (2, HintGrouping.One)},

			{new Vector2(0, 8) , (0, HintGrouping.One)},
			{new Vector2(1, 8) , (1, HintGrouping.One)},
			{new Vector2(2, 8) , (1, HintGrouping.One)},
			{new Vector2(4, 8) , (1, HintGrouping.One)},
			{new Vector2(5, 8) , (0, HintGrouping.One)},
			{new Vector2(7, 8) , (3, HintGrouping.Two)},
			{new Vector2(9, 8) , (8, HintGrouping.Two)},

			{new Vector2(1, 7) , (2, HintGrouping.Two)},
			{new Vector2(3, 7) , (5, HintGrouping.Two)},
			{new Vector2(5, 7) , (3, HintGrouping.Two)},
			{new Vector2(6, 7) , (4, HintGrouping.ThreePlus)},
			{new Vector2(8, 7) , (5, HintGrouping.Two)},
			{new Vector2(9, 7) , (6, HintGrouping.Two)},

			{new Vector2(0, 6) , (2, HintGrouping.Two)},
			{new Vector2(1, 6) , (3, HintGrouping.Two)},
			{new Vector2(3, 6) , (4, HintGrouping.One)},
			{new Vector2(4, 6) , (3, HintGrouping.One)},
			{new Vector2(6, 6) , (6, HintGrouping.One)},
			{new Vector2(7, 6) , (7, HintGrouping.One)},
			{new Vector2(8, 6) , (6, HintGrouping.One)},
			{new Vector2(9, 6) , (6, HintGrouping.One)},
			
			{new Vector2(0, 5) , (3, HintGrouping.Two)},
			{new Vector2(1, 5) , (2, HintGrouping.Two)},
			{new Vector2(2, 5) , (3, HintGrouping.Two)},
			{new Vector2(3, 5) , (4, HintGrouping.One)},
			{new Vector2(4, 5) , (4, HintGrouping.One)},
			{new Vector2(5, 5) , (4, HintGrouping.ThreePlus)},
			{new Vector2(6, 5) , (4, HintGrouping.ThreePlus)},
			{new Vector2(7, 5) , (5, HintGrouping.One)},
			{new Vector2(8, 5) , (4, HintGrouping.One)},
			{new Vector2(9, 5) , (4, HintGrouping.One)},

			{new Vector2(0, 4) , (1, HintGrouping.One)},
			{new Vector2(1, 4) , (0, HintGrouping.One)},
			{new Vector2(2, 4) , (0, HintGrouping.One)},
			{new Vector2(4, 4) , (2, HintGrouping.Two)},
			{new Vector2(5, 4) , (2, HintGrouping.One)},
			{new Vector2(7, 4) , (7, HintGrouping.One)},
			{new Vector2(8, 4) , (5, HintGrouping.One)},
			{new Vector2(9, 4) , (4, HintGrouping.One)},

			{new Vector2(1, 3) , (0, HintGrouping.One)},
			{new Vector2(3, 3) , (1, HintGrouping.One)},
			{new Vector2(4, 3) , (1, HintGrouping.One)},
			{new Vector2(5, 3) , (3, HintGrouping.Two)},
			{new Vector2(6, 3) , (0, HintGrouping.One)},
			{new Vector2(8, 3) , (7, HintGrouping.Two)},

			{new Vector2(1, 2) , (0, HintGrouping.One)},
			{new Vector2(2, 2) , (2, HintGrouping.One)},
			{new Vector2(4, 2) , (3, HintGrouping.One)},
			{new Vector2(6, 2) , (2, HintGrouping.One)},
			{new Vector2(8, 2) , (3, HintGrouping.One)},
			{new Vector2(9, 2) , (7, HintGrouping.Two)},

			{new Vector2(1, 1) , (1, HintGrouping.One)},
			{new Vector2(2, 1) , (2, HintGrouping.One)},
			{new Vector2(4, 1) , (3, HintGrouping.One)},
			{new Vector2(5, 1) , (4, HintGrouping.One)},
			{new Vector2(7, 1) , (5, HintGrouping.One)},
			{new Vector2(9, 1) , (5, HintGrouping.Two)},
			
			{new Vector2(1, 0) , (0, HintGrouping.One)},
			{new Vector2(3, 0) , (2, HintGrouping.One)},
			{new Vector2(4, 0) , (2, HintGrouping.One)},
			{new Vector2(5, 0) , (3, HintGrouping.One)},
			{new Vector2(6, 0) , (3, HintGrouping.One)},
			{new Vector2(7, 0) , (4, HintGrouping.One)},
			{new Vector2(8, 0) , (2, HintGrouping.One)},
			{new Vector2(9, 0) , (2, HintGrouping.One)},

		};
	private readonly ManagedDict zHints =
		new ManagedDict {
			
			{new Vector2(0, 0) , (3, HintGrouping.One)},
			{new Vector2(1, 0) , (1, HintGrouping.One)},
			{new Vector2(3, 0) , (4, HintGrouping.Two)},
			{new Vector2(4, 0) , (2, HintGrouping.Two)},
			{new Vector2(6, 0) , (2, HintGrouping.Two)},
			{new Vector2(7, 0) , (4, HintGrouping.Two)},
			{new Vector2(8, 0) , (4, HintGrouping.Two)},
			
			{new Vector2(0, 1) , (2, HintGrouping.One)},
			{new Vector2(1, 1) , (1, HintGrouping.One)},
			{new Vector2(2, 1) , (3, HintGrouping.One)},
			{new Vector2(3, 1) , (3, HintGrouping.One)},
			{new Vector2(4, 1) , (3, HintGrouping.One)},
			{new Vector2(6, 1) , (3, HintGrouping.Two)},
			{new Vector2(9, 1) , (5, HintGrouping.Two)},
			
			{new Vector2(0, 2) , (2, HintGrouping.One)},
			{new Vector2(2, 2) , (1, HintGrouping.One)},
			{new Vector2(3, 2) , (3, HintGrouping.One)},
			{new Vector2(4, 2) , (3, HintGrouping.One)},
			{new Vector2(5, 2) , (3, HintGrouping.One)},
			{new Vector2(6, 2) , (5, HintGrouping.ThreePlus)},
			{new Vector2(7, 2) , (8, HintGrouping.One)},
			{new Vector2(8, 2) , (8, HintGrouping.One)},
			{new Vector2(9, 2) , (8, HintGrouping.One)},
			
			{new Vector2(0, 3) , (0, HintGrouping.One)},
			{new Vector2(2, 3) , (0, HintGrouping.One)},
			{new Vector2(3, 3) , (0, HintGrouping.One)},
			{new Vector2(5, 3) , (3, HintGrouping.One)},
			{new Vector2(6, 3) , (6, HintGrouping.Two)},
			{new Vector2(7, 3) , (9, HintGrouping.One)},
			{new Vector2(8, 3) , (9, HintGrouping.One)},
			{new Vector2(9, 3) , (8, HintGrouping.One)},

			{new Vector2(0, 4) , (0, HintGrouping.One)},
			{new Vector2(1, 4) , (3, HintGrouping.One)},
			{new Vector2(2, 4) , (3, HintGrouping.One)},
			{new Vector2(5, 4) , (1, HintGrouping.One)},
			{new Vector2(6, 4) , (3, HintGrouping.One)},
			{new Vector2(7, 4) , (7, HintGrouping.Two)},

			{new Vector2(0, 5) , (0, HintGrouping.One)},
			{new Vector2(2, 5) , (3, HintGrouping.One)},
			{new Vector2(3, 5) , (3, HintGrouping.One)},
			{new Vector2(5, 5) , (1, HintGrouping.One)},
			{new Vector2(6, 5) , (1, HintGrouping.One)},
			{new Vector2(7, 5) , (5, HintGrouping.Two)},
			{new Vector2(8, 5) , (3, HintGrouping.Two)},
			{new Vector2(9, 5) , (2, HintGrouping.Two)},
			
			{new Vector2(0, 6) , (2, HintGrouping.Two)},
			{new Vector2(3, 6) , (3, HintGrouping.One)},
			{new Vector2(4, 6) , (3, HintGrouping.One)},
			{new Vector2(5, 6) , (3, HintGrouping.One)},
			{new Vector2(6, 6) , (3, HintGrouping.One)},
			{new Vector2(7, 6) , (5, HintGrouping.Two)},
			{new Vector2(8, 6) , (3, HintGrouping.ThreePlus)},
			{new Vector2(9, 6) , (6, HintGrouping.Two)},

			{new Vector2(0, 7) , (3, HintGrouping.One)},
			{new Vector2(1, 7) , (3, HintGrouping.One)},
			{new Vector2(2, 7) , (3, HintGrouping.One)},
			{new Vector2(3, 7) , (3, HintGrouping.One)},
			{new Vector2(5, 7) , (5, HintGrouping.ThreePlus)},
			{new Vector2(6, 7) , (1, HintGrouping.One)},
			{new Vector2(7, 7) , (2, HintGrouping.Two)},
			{new Vector2(8, 7) , (2, HintGrouping.Two)},
			{new Vector2(9, 7) , (5, HintGrouping.Two)},
			
			{new Vector2(2, 8) , (1, HintGrouping.One)},
			{new Vector2(3, 8) , (3, HintGrouping.One)},
			{new Vector2(4, 8) , (5, HintGrouping.ThreePlus)},
			{new Vector2(8, 8) , (2, HintGrouping.Two)},
			
			{new Vector2(0, 9) , (0, HintGrouping.One)},
			{new Vector2(2, 9) , (0, HintGrouping.One)},
			{new Vector2(3, 9) , (0, HintGrouping.One)},
			{new Vector2(4, 9) , (2, HintGrouping.Two)},
			{new Vector2(6, 9) , (2, HintGrouping.Two)},
			{new Vector2(7, 9) , (0, HintGrouping.One)},
			{new Vector2(8, 9) , (0, HintGrouping.One)},
			{new Vector2(9, 9) , (2, HintGrouping.Two)},

		};
	private Vector2 _mouseMotion = Vector2.Zero;

	private int cull = 0;
	// false is x, true is z
	private bool cullDirection = false;
	
	// solving
	private int currentRowHighlight = 0;
	private int scanClock = 0;
	private int scanCycle = 15;
	private bool continueScan = false;

	private NDArray View
	{
		get
		{
			var x = cullDirection ? ":" : $"{cull}:";
			var z = cullDirection ? $"{cull}:" : ":";
			return _data[$"{x},:,{z},:"];
		}
		
	}

	private NDArray Data
	{
		get
		{
			
			var subtracted = 1000 - (Mathf.Abs(cull) * 100);
			return View.reshape(subtracted, 4);
		}
	}
	
	#region Godot Object overrides
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		_data = np.ones(10, 10, 10, 4);
		for (var x = 0; x < 10; x++)
		{
			for (var y = 0; y < 10; y++)
			{
				for (var z = 0; z < 10; z++)
				{
					_data[x, y, z, 1] = x;
					_data[x, y, z, 2] = y;
					_data[x, y, z, 3] = z;
				}
			}
		}

		// var thread = new Thread();
		// thread.Start(this, "")
		
		GenerateChunkMesh();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

		 if (Input.IsActionJustPressed("ui_cancel"))
			 Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured
				 ? Input.MouseModeEnum.Visible
				 : Input.MouseModeEnum.Captured;
		 
		 var xAngle = ((2 * Mathf.Pi) / 1000) * _mouseMotion.x;
		 RotateY(xAngle);
		 
		 _mouseMotion *= 0.9f;

		 if (continueScan)
		 {
			 scanClock = ++scanClock % scanCycle;
			 if (scanClock == 0)
			 {
				 currentRowHighlight = ++currentRowHighlight % 10;
				 if (currentRowHighlight == 0)
				 {
					 cull = ++cull % 10;
					 if (cull == 0)
					 {
						 cullDirection = !cullDirection;
					 }
				 } 
				 Regenerate();
				 var hintDict = cullDirection ? xHints : zHints;
				 if (hintDict.TryGetValue(new Vector2(cull, currentRowHighlight), out var hint))
				 {
					 var x = cullDirection ? ":" : $"{cull}";
					 var z = cullDirection ? $"{cull}" : ":";
					 var slice = _data[$"{x},{currentRowHighlight},{z},0"];
					 var doubles = slice.ToArray<double>();
					 var row = new Array<int>(from d in doubles select (int)d);
					 var change = ReduceRow(hint, in row);
				 }
			 }
		 }
		 
		 

	}

	private bool ReduceRow((int, HintGrouping) hint, in Array<int> row)
	{
		return false;
	}
 
	public override void _Input(InputEvent e) {
		PuzzleInput.OnPuzzleTurn(e, motion =>
		{
			_mouseMotion = motion;
		});
		
		if (Input.IsActionJustPressed("ui_accept"))
			continueScan = !continueScan;

		if (Input.IsActionJustPressed("ui_up"))
			scanCycle += 5;

		if (Input.IsActionJustPressed("ui_down"))
			scanCycle = Math.Max(5, scanCycle - 5);
	}
	
	#endregion

	#region GenerateChunkMesh tree
	
	// TODO rename probably
	private Array<Vector3> GetFilledCells()
	{

		var result = Data
			.GetNDArrays()
			.Where(a => a.GetDouble(0) > 0)
			.Select(a =>
			{
				var cell = a.ToArray<double>().Select(d => (float)d).ToArray();
				var x = cell[1];
				var y = cell[2];
				var z = cell[3];
				var vec = new Vector3(x, y, z);
				return vec;
			});
		
		return new Array<Vector3>(result);
	}
	
	#region DrawBlockMesh tree

	private static Array CalculateBlockVerts(Vector3 blockSubPosition)
	{
		// TODO is offset when puzzle dimension has odd length
		// - Vector3.One * 0.5f;
		var blockPosition = blockSubPosition - Vector3.One * 5;
		return new Array(
			new Vector3(blockPosition.x, blockPosition.y, blockPosition.z),
			new Vector3(blockPosition.x, blockPosition.y, blockPosition.z + 1),
			new Vector3(blockPosition.x, blockPosition.y + 1, blockPosition.z),
			new Vector3(blockPosition.x, blockPosition.y + 1, blockPosition.z + 1),
			new Vector3(blockPosition.x + 1, blockPosition.y, blockPosition.z),
			new Vector3(blockPosition.x + 1, blockPosition.y, blockPosition.z + 1),
			new Vector3(blockPosition.x + 1, blockPosition.y + 1, blockPosition.z),
			new Vector3(blockPosition.x + 1, blockPosition.y + 1, blockPosition.z + 1)
		);
	}
	
	private Array CalculateBlockUvs(ManagedDict hints, Vector2 position, bool isMarked, bool isHighlighted)
	{
		int tile;

		if (hints.TryGetValue(position, out var pair))
		{
			var face = pair.Item1;
			var grouping = (int)pair.Item2;
			tile = grouping * 3 * 10 + face;

			if (isHighlighted)
			{
				tile += 20;
			}
			else if (isMarked)
			{
				tile += 10;
			}
		}
		else
		{
			tile = isHighlighted ? 92 : isMarked ? 91 : 90;
		}

		var row = tile / TextureSheetWidth;
		var col = tile % TextureSheetWidth;

		return new Array(
			TextureTileSize * new Vector2(col, row),
			TextureTileSize * new Vector2(col, row +1),
			TextureTileSize * new Vector2(col +1, row),
			TextureTileSize * new Vector2(col +1, row + 1)
		);
	}
	
	#region IsFullCell tree
	
	bool IsOutOfBounds(Vector3 v)
	{
		
		var xCull = cullDirection ? 0 : cull;
		var zCull = cullDirection ? cull : 0;
		return xCull > v.x || 0 > v.y || zCull > v.z
		       || v.x >= 10
		       || v.y >= 10
		       || v.z >= 10;

	}
	
	private bool IsFullCell(Vector3 location)
	{
		
		if (IsOutOfBounds(location)) return false;

		var cell = _data[
			(int)location.x,
			(int)location.y,
			(int)location.z
		];

		// Have to use GetDouble because NDArray is typed as double. Any other Get results in an NPE.
		var cellState = cell.GetDouble(0);
		return cellState > 0;

	}

	private (bool isMarked, bool isHinted) IsMarkedOrHighlightedCell(Vector3 location)
	{
		var cell = _data[
			(int)location.x,
			(int)location.y,
			(int)location.z
		];

		// Have to use GetDouble because NDArray is typed as double. Any other Get results in an NPE.
		var cellState = cell.GetDouble(0);
		var isMarked = cellState > 1;

		bool isHighlighted;
		
		// culling z, check x
		if (cullDirection)
		{
			isHighlighted = (int)location.y == currentRowHighlight && (int)location.z == cull;
		}
		// culling x, check z
		else
		{
			isHighlighted = (int)location.y == currentRowHighlight && (int)location.x == cull;
		}

		return (isMarked, isHighlighted);
	}
	
	#endregion
	
	private static void DrawBlockFace(SurfaceTool surfaceTool, Array verts, Array uvs)
	{
		surfaceTool.AddUv((Vector2)uvs[1]);
		surfaceTool.AddVertex((Vector3)verts[1]);
		surfaceTool.AddUv((Vector2)uvs[2]);
		surfaceTool.AddVertex((Vector3)verts[2]);
		surfaceTool.AddUv((Vector2)uvs[3]);
		surfaceTool.AddVertex((Vector3)verts[3]);

		surfaceTool.AddUv((Vector2)uvs[2]);
		surfaceTool.AddVertex((Vector3)verts[2]);
		surfaceTool.AddUv((Vector2)uvs[1]);
		surfaceTool.AddVertex((Vector3)verts[1]);
		surfaceTool.AddUv((Vector2)uvs[0]);
		surfaceTool.AddVertex((Vector3)verts[0]);
	}
	
	private void DrawBlockMesh(SurfaceTool surfaceTool, Vector3 blockSubPosition, bool value)
	{

		var verts = CalculateBlockVerts(blockSubPosition);
		var (isMarked, isHighlighted) = IsMarkedOrHighlightedCell(blockSubPosition);

		var xUvs = CalculateBlockUvs(xHints, new Vector2(blockSubPosition.z, blockSubPosition.y), isMarked, isHighlighted);
		var yUvs = CalculateBlockUvs(yHints, new Vector2(blockSubPosition.x, blockSubPosition.z), isMarked, isHighlighted);
		var zUvs = CalculateBlockUvs(zHints, new Vector2(blockSubPosition.x, blockSubPosition.y), isMarked, isHighlighted);

		if (!IsFullCell(blockSubPosition + Vector3.Left))
			DrawBlockFace(surfaceTool, new Array(verts[2], verts[0], verts[3], verts[1]), xUvs);
		
		if (!IsFullCell(blockSubPosition + Vector3.Right))
			DrawBlockFace(surfaceTool, new Array(verts[7], verts[5], verts[6], verts[4]), xUvs);
		
		if (!IsFullCell(blockSubPosition + Vector3.Forward))
			DrawBlockFace(surfaceTool, new Array(verts[6], verts[4], verts[2], verts[0]), zUvs);
		
		if (!IsFullCell(blockSubPosition + Vector3.Back))
			DrawBlockFace(surfaceTool, new Array(verts[3], verts[1], verts[7], verts[5]), zUvs);
		
		if (!IsFullCell(blockSubPosition + Vector3.Down))
			DrawBlockFace(surfaceTool, new Array(verts[4], verts[5], verts[0], verts[1]), yUvs);
		
		if (!IsFullCell(blockSubPosition + Vector3.Up))
			DrawBlockFace(surfaceTool, new Array(verts[2], verts[3], verts[6], verts[7]), yUvs);
	}
	
	#endregion

	private void GenerateChunkMesh()
	{

		// TODO might need
		// if (data.Count == 0) return;

		var surfaceTool = new SurfaceTool();
		surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		// For each block, add data to the SurfaceTool and generate a collider.
		foreach (var location in GetFilledCells())
		{
			DrawBlockMesh(surfaceTool, location, true);
		}

		// # Create the chunk's mesh from the SurfaceTool data.
		surfaceTool.GenerateNormals();
		surfaceTool.GenerateTangents();
		surfaceTool.Index();
		var arrayMesh = surfaceTool.Commit();
		var mi = new MeshInstance();
		mi.Mesh = arrayMesh;
		mi.MaterialOverride = ResourceLoader.Load<Material>("res://resource/material.tres");

		AddChild(mi);
	}
	
	#endregion

	void Regenerate()
	{
		foreach (Node c in GetChildren())
		{
			RemoveChild(c);
			c.QueueFree();
		}
		GenerateChunkMesh();
		//GenerateChunkCollider
	}
	
}
