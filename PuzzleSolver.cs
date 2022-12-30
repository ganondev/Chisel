using Godot;
using System;
using System.Collections.Specialized;
using System.Linq;
using Chisel;
using Godot.Collections;
using NumSharp;
using Array = Godot.Collections.Array;

public class PuzzleSolver : StaticBody
{
	private const int TEXTURE_SHEET_WIDTH = 8;
	private const float TEXTURE_TILE_SIZE = (float)(1.0 / TEXTURE_SHEET_WIDTH) - 0.01f;
		
	private NDArray data;
	// private readonly Godot.Collections.Dictionary<Vector3, bool> data = new Godot.Collections.Dictionary<Vector3, bool>();
	private readonly Godot.Collections.Dictionary<Vector3, bool> hints;

	Array<Vector3> getFilledCells()
	{
		// var result =
		// 	from cell in data.reshape(1000, 4).GetNDArrays()
		// 	where (int)cell[0] == 1
		// 	select new Vector3(cell[1], cell[2], cell[3]);

		var result = data
			.reshape(1000, 4)
			.GetNDArrays()
			.Where(a =>
			{
				return a[0] == 1;
			})
			.Select(a =>
			{
				var cell = a.ToArray<double>().Select(d => (float)d).ToArray();
				float x = cell[1];
				float y = cell[2];
				float z = cell[3];
				var vec = new Vector3(x, y, z);
				// GD.Print($"{cell} {vec} {x} {y} {z}");
				return vec;
			});
		// GD.Print(result.ToArray().Length);
		
		// GD.Print(new Array<Vector3>(result));
		return new Array<Vector3>(result);
	}

	bool IsOutOfBounds(Vector3 v)
	{
		return -5 > v.x || v.x >= 5 || -5 > v.y || v.y >= 5 || -5 > v.z || v.z >= 5;
	}

	bool isFullCell(Vector3 location)
	{

		if (IsOutOfBounds(location)) return false;

		var cell = data[
			(int)location.x + 5,
			(int)location.y + 5,
			(int)location.z + 5
		];
		
		// GD.Print(location);
		var ret = cell[0] == 1;
		return ret;

	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		data = np.ones(10, 10, 10, 4);
		for (int x = 0; x < 10; x++)
		{
			for (int y = 0; y < 10; y++)
			{
				for (int z = 0; z < 10; z++)
				{
					data[x, y, z, 1] = x - 5;
					data[x, y, z, 2] = y - 5;
					data[x, y, z, 3] = z - 5;
				}
			}
		}

		// Input.MouseMode = Input.MouseModeEnum.Captured;

		// var thread = new Thread();
		// thread.Start(this, "")
		
		_generate_chunk_mesh();

	}

	void _generate_chunk_mesh()
	{
		
		// TODO might need
		// if (data.Count == 0) return;

		var surfaceTool = new SurfaceTool();
		surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		// For each block, add data to the SurfaceTool and generate a collider.
		foreach (var location in getFilledCells())
		{
			DrawBlockMesh(surfaceTool, location, true);
		}

		// # Create the chunk's mesh from the SurfaceTool data.
		surfaceTool.GenerateNormals();
		surfaceTool.GenerateTangents();
		surfaceTool.Index();
		var array_mesh = surfaceTool.Commit();
		var mi = new MeshInstance();
		mi.Mesh = array_mesh;
		// mi.MaterialOverride = preload("res://world/textures/material.tres")
		mi.MaterialOverride = ResourceLoader.Load<Material>("res://resource/material.tres");
			
		AddChild(mi);
	}

	void DrawBlockMesh(SurfaceTool surfaceTool, Vector3 blockSubPosition, bool value)
	{
		
		var verts = calculate_block_verts(blockSubPosition);
		var uvs = calculate_block_uvs(value);
		var topUvs = uvs;
		var bottomUvs = uvs;


		// // # Allow some blocks to have different top/bottom textures.
		// 		if block_id == 3: # Grass.top_uvs = calculate_block_uvs(0)
		// 		bottom_uvs = calculate_block_uvs(2)
		// 		elif block_id == 5: # Furnace.top_uvs = calculate_block_uvs(31)
		// 		bottom_uvs = top_uvs
		// 		elif block_id == 12: # Log.top_uvs = calculate_block_uvs(30)
		// 		bottom_uvs = top_uvs
		// 		elif block_id == 19: # Bookshelf.top_uvs = calculate_block_uvs(4)
		// 		bottom_uvs = top_uvs


		if (!isFullCell(blockSubPosition + Vector3.Left))
			_draw_block_face(surfaceTool, new Array(verts[2], verts[0], verts[3], verts[1]), uvs);
		
		if (!isFullCell(blockSubPosition + Vector3.Right))
			_draw_block_face(surfaceTool, new Array(verts[7], verts[5], verts[6], verts[4]), uvs);
		
		if (!isFullCell(blockSubPosition + Vector3.Forward))
			_draw_block_face(surfaceTool, new Array(verts[6], verts[4], verts[2], verts[0]), uvs);
		
		if (!isFullCell(blockSubPosition + Vector3.Back))
			_draw_block_face(surfaceTool, new Array(verts[3], verts[1], verts[7], verts[5]), uvs);
		
		if (!isFullCell(blockSubPosition + Vector3.Down))
			_draw_block_face(surfaceTool, new Array(verts[4], verts[5], verts[0], verts[1]), bottomUvs);
		
		if (!isFullCell(blockSubPosition + Vector3.Up))
			_draw_block_face(surfaceTool, new Array(verts[2], verts[3], verts[6], verts[7]), topUvs);
	}
	
	void _draw_block_face(SurfaceTool surfaceTool, Array verts, Array uvs)
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

	static Array calculate_block_uvs(bool val) {
		// # This method only supports square texture sheets.
		// const int blockId = 1;
		var r = new Random();
		var blockId = r.Next(1, 10);
		var row = 0;//blockId / TEXTURE_SHEET_WIDTH;
		var col = 0;//blockId % TEXTURE_SHEET_WIDTH;

		return new Array(
			TEXTURE_TILE_SIZE * new Vector2(col, row),
			TEXTURE_TILE_SIZE * new Vector2(col, row +1),
			TEXTURE_TILE_SIZE * new Vector2(col +1, row),
			TEXTURE_TILE_SIZE * new Vector2(col +1, row + 1)
		);
	}
	
	static Array calculate_block_verts(Vector3 blockSubPosition)
	{
		var blockPosition = blockSubPosition;// - Vector3.One * 0.5f;
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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{

		 if (Input.IsActionJustPressed("ui_cancel"))
			 Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured
				 ? Input.MouseModeEnum.Visible
				 : Input.MouseModeEnum.Captured;
		 
		 var xAngle = ((2 * Mathf.Pi) / 1000) * mouseMotion.x;
		 RotateY(xAngle);
		 
		 mouseMotion *= 0.9f;

	}

	private Vector2 mouseMotion = Vector2.Zero;
	
	public override void _Input(InputEvent e) {
		PuzzleInput.OnPuzzleTurn(e, (motion) =>
		{
			mouseMotion = motion;
		});
	}
	
}
