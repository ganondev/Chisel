using Godot;
using System;
using Godot.Collections;
using Array = Godot.Collections.Array;

public class Puzzle : StaticBody
{
	private const int TEXTURE_SHEET_WIDTH = 8;
	private const float TEXTURE_TILE_SIZE = (float)(1.0 / TEXTURE_SHEET_WIDTH);
		
	private readonly Dictionary<Vector3, bool> data = new Dictionary<Vector3, bool>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		Input.MouseMode = Input.MouseModeEnum.Captured;

		data[Vector3.Zero] = true;
		data[Vector3.Up] = true;
		data[Vector3.Down] = true;
		data[Vector3.Left] = true;
		data[Vector3.Right] = true;
		data[Vector3.Forward] = true;
		data[Vector3.Back] = true;

		// var thread = new Thread();
		// thread.Start(this, "")
		
		_generate_chunk_mesh();

	}

	void _generate_chunk_mesh()
	{

		if (data.Count == 0) return;

		var surfaceTool = new SurfaceTool();
		surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

		// For each block, add data to the SurfaceTool and generate a collider.
		// for blockPosition in
		// data.keys():
		// var block_id = data[blockPosition]
		// _draw_block_mesh(surfaceTool, blockPosition, block_id)

		foreach (var pair in data)
		{
			_draw_block_mesh(surfaceTool, pair.Key - Vector3.One * 0.5f, pair.Value);
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

	void _draw_block_mesh(SurfaceTool surfaceTool, Vector3 blockSubPosition, bool value)
	{
		var verts = calculate_block_verts(blockSubPosition);
		var uvs = calculate_block_uvs(value);
		var top_uvs = uvs;
		var bottom_uvs = uvs;

// // # Allow some blocks to have different top/bottom textures.
// 		if block_id == 3: # Grass.top_uvs = calculate_block_uvs(0)
// 		bottom_uvs = calculate_block_uvs(2)
// 		elif block_id == 5: # Furnace.top_uvs = calculate_block_uvs(31)
// 		bottom_uvs = top_uvs
// 		elif block_id == 12: # Log.top_uvs = calculate_block_uvs(30)
// 		bottom_uvs = top_uvs
// 		elif block_id == 19: # Bookshelf.top_uvs = calculate_block_uvs(4)
// 		bottom_uvs = top_uvs

// # Main rendering code for normal blocks.
		data.TryGetValue(blockSubPosition + Vector3.Left, out var hasLeft);
		if (!hasLeft)
			_draw_block_face(surfaceTool, new Array(verts[2], verts[0], verts[3], verts[1]), uvs);

		// otherBlockPosition = blockSubPosition + Vector3.Right;
		// otherBlockId = 0;
		// if (otherBlockPosition.x == CHUNK_SIZE):
		// otherBlockId = voxel_world.get_block_global_position(otherBlockPosition + chunk_position * CHUNK_SIZE)
		// elif data.has(otherBlockPosition):
		// otherBlockId = data[otherBlockPosition]
		// if block_id != otherBlockId and is_block_transparent(otherBlockId):
		data.TryGetValue(blockSubPosition + Vector3.Right, out var hasRight);
		if (!hasRight)
			_draw_block_face(surfaceTool, new Array(verts[7], verts[5], verts[6], verts[4]), uvs);

		// otherBlockPosition = blockSubPosition + Vector3.FORWARD
		// otherBlockId = 0
		// if otherBlockPosition.z == -1:
		// otherBlockId = voxel_world.get_block_global_position(otherBlockPosition + chunk_position * CHUNK_SIZE)
		// elif data.has(otherBlockPosition):
		// otherBlockId = data[otherBlockPosition]
		// if block_id != otherBlockId and is_block_transparent(otherBlockId):
		data.TryGetValue(blockSubPosition + Vector3.Forward, out var hasForward);
		if (!hasForward)
			_draw_block_face(surfaceTool, new Array(verts[6], verts[4], verts[2], verts[0]), uvs);

		// otherBlockPosition = blockSubPosition + Vector3.BACK
		// otherBlockId = 0
		// if otherBlockPosition.z == CHUNK_SIZE:
		// otherBlockId = voxel_world.get_block_global_position(otherBlockPosition + chunk_position * CHUNK_SIZE)
		// elif data.has(otherBlockPosition):
		// otherBlockId = data[otherBlockPosition]
		// if block_id != otherBlockId and is_block_transparent(otherBlockId):
		data.TryGetValue(blockSubPosition + Vector3.Back, out var hasBack);
		if (!hasBack)
			_draw_block_face(surfaceTool, new Array(verts[3], verts[1], verts[7], verts[5]), uvs);

		// otherBlockPosition = blockSubPosition + Vector3.DOWN
		// otherBlockId = 0
		// if otherBlockPosition.y == -1:
		// otherBlockId = voxel_world.get_block_global_position(otherBlockPosition + chunk_position * CHUNK_SIZE)
		// elif data.has(otherBlockPosition):
		// otherBlockId = data[otherBlockPosition]
		// if block_id != otherBlockId and is_block_transparent(otherBlockId):
		data.TryGetValue(blockSubPosition + Vector3.Down, out var hasDown);
		if (!hasDown)
			_draw_block_face(surfaceTool, new Array(verts[4], verts[5], verts[0], verts[1]), bottom_uvs);

		// otherBlockPosition = blockSubPosition + Vector3.UP
		// otherBlockId = 0
		// if otherBlockPosition.y == CHUNK_SIZE:
		// otherBlockId = voxel_world.get_block_global_position(otherBlockPosition + chunk_position * CHUNK_SIZE)
		// elif data.has(otherBlockPosition):
		// otherBlockId = data[otherBlockPosition]
		// if block_id != otherBlockId and is_block_transparent(otherBlockId):
		data.TryGetValue(blockSubPosition + Vector3.Up, out var hasUp);
		if (!hasUp)
			_draw_block_face(surfaceTool, new Array(verts[2], verts[3], verts[6], verts[7]), top_uvs);
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
		var row = blockId / TEXTURE_SHEET_WIDTH;
		var col = blockId % TEXTURE_SHEET_WIDTH;

		return new Array(
			TEXTURE_TILE_SIZE * new Vector2(col, row),
			TEXTURE_TILE_SIZE * new Vector2(col, row +1),
			TEXTURE_TILE_SIZE * new Vector2(col +1, row),
			TEXTURE_TILE_SIZE * new Vector2(col +1, row + 1)
		);
				
	}
	
	static Array calculate_block_verts(Vector3 blockPosition){
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

		// var newTransform = Transform;
		// newTransform.basis = new Basis(new Vector3(0, mouseMotion.x * -.001f, 0));
		//
		// Transform = newTransform;
		// Transform.

		// Transform = Transform.Rotated(Vector3.Up, );
		var angle = ((2 * Mathf.Pi) / 1000) * mouseMotion.x;
		RotateY(angle);
		GD.Print("Rotated by ", angle);
		mouseMotion *= 0.9f;

	}

	private Vector2 mouseMotion = Vector2.Zero;
	
	public override void _Input(InputEvent e) {
		if (e is InputEventMouseMotion motion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			mouseMotion = motion.Relative;
			// GD.Print(mouseMotion);
		}
	}
	
}
