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
		new ManagedDict{
			
		};
	private readonly ManagedDict yHints =
		new ManagedDict {
			{new Vector2(0, 0) , (0, HintGrouping.One)},
			{new Vector2(9, 0) , (0, HintGrouping.One)},
			{new Vector2(0, 9) , (0, HintGrouping.One)},
			{new Vector2(9, 9) , (0, HintGrouping.One)},
		};
	private readonly ManagedDict zHints =
		new ManagedDict {
			{new Vector2(0, 0) , (0, HintGrouping.One)},
			{new Vector2(1, 0) , (1, HintGrouping.One)},
			{new Vector2(2, 0) , (2, HintGrouping.One)},
			{new Vector2(3, 0) , (3, HintGrouping.One)},
			{new Vector2(4, 0) , (4, HintGrouping.One)},
			{new Vector2(5, 0) , (5, HintGrouping.One)},
			{new Vector2(6, 0) , (6, HintGrouping.One)},
			{new Vector2(7, 0) , (7, HintGrouping.One)},
			{new Vector2(8, 0) , (8, HintGrouping.One)},
			{new Vector2(9, 0) , (9, HintGrouping.One)},
			{new Vector2(0, 1) , (0, HintGrouping.Two)},
			{new Vector2(1, 1) , (1, HintGrouping.Two)},
			{new Vector2(2, 1) , (2, HintGrouping.Two)},
			{new Vector2(3, 1) , (3, HintGrouping.Two)},
			{new Vector2(4, 1) , (4, HintGrouping.Two)},
			{new Vector2(5, 1) , (5, HintGrouping.Two)},
			{new Vector2(6, 1) , (6, HintGrouping.Two)},
			{new Vector2(7, 1) , (7, HintGrouping.Two)},
			{new Vector2(8, 1) , (8, HintGrouping.Two)},
			{new Vector2(9, 1) , (9, HintGrouping.Two)},
			{new Vector2(0, 2) , (0, HintGrouping.ThreePlus)},
			{new Vector2(1, 2) , (1, HintGrouping.ThreePlus)},
			{new Vector2(2, 2) , (2, HintGrouping.ThreePlus)},
			{new Vector2(3, 2) , (3, HintGrouping.ThreePlus)},
			{new Vector2(4, 2) , (4, HintGrouping.ThreePlus)},
			{new Vector2(5, 2) , (5, HintGrouping.ThreePlus)},
			{new Vector2(6, 2) , (6, HintGrouping.ThreePlus)},
			{new Vector2(7, 2) , (7, HintGrouping.ThreePlus)},
			{new Vector2(8, 2) , (8, HintGrouping.ThreePlus)},
			{new Vector2(9, 2) , (9, HintGrouping.ThreePlus)},
		};
	private Vector2 _mouseMotion = Vector2.Zero;

	#region Godot Object overrides
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		_data = np.ones(10, 10, 10, 4);
		for (int x = 0; x < 10; x++)
		{
			for (int y = 0; y < 10; y++)
			{
				for (int z = 0; z < 10; z++)
				{
					_data[x, y, z, 1] = x - 5;
					_data[x, y, z, 2] = y - 5;
					_data[x, y, z, 3] = z - 5;
				}
			}
		}

		// Input.MouseMode = Input.MouseModeEnum.Captured;

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

	}

	public override void _Input(InputEvent e) {
		PuzzleInput.OnPuzzleTurn(e, (motion) =>
		{
			_mouseMotion = motion;
		});
	}
	
	#endregion

	#region GenerateChunkMesh tree
	
	private Array<Vector3> GetFilledCells()
	{

		var result = _data
			.reshape(1000, 4)
			.GetNDArrays()
			.Where(a => a[0] == 1)
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
		var blockPosition = blockSubPosition;
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
	
	private static Array CalculateBlockUvs(ManagedDict hints, Vector2 position)
	{
		position += new Vector2(5, 5);
		int tile;
		if (hints.TryGetValue(position, out var pair))
		{
			var face = pair.Item1;
			var grouping = (int)pair.Item2;
			tile = grouping * 2 * 10 + face;
		}
		else
		{
			tile = 60;
		}

		if (new Random().Next(2) == 0)
		{
			tile += 10;
		}

		int row = tile / TextureSheetWidth;
		int col = tile % TextureSheetWidth;

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
		return -5 > v.x || v.x >= 5 || -5 > v.y || v.y >= 5 || -5 > v.z || v.z >= 5;
	}
	
	private bool IsFullCell(Vector3 location)
	{

		if (IsOutOfBounds(location)) return false;

		var cell = _data[
			(int)location.x + 5,
			(int)location.y + 5,
			(int)location.z + 5
		];
		
		var ret = cell[0] == 1;
		return ret;

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

		var xUvs = CalculateBlockUvs(xHints, new Vector2(blockSubPosition.z, blockSubPosition.y));
		var yUvs = CalculateBlockUvs(yHints, new Vector2(blockSubPosition.x, blockSubPosition.z));
		var zUvs = CalculateBlockUvs(zHints, new Vector2(blockSubPosition.x, blockSubPosition.y));

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

}
