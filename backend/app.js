require("dotenv").config();
const express = require("express");
const mongoose = require("mongoose");
const cors = require("cors");
const bodyParser = require("body-parser");

const playerRoutes = require("./routes/playerRoutes");

const app = express();
app.use(express.json());
app.use(cors());
app.use(bodyParser.json());


const mongoURI = process.env.MONGO_URI;

// Connect to MongoDB
mongoose.connect(mongoURI, 
  {
    useNewUrlParser: true,
    useUnifiedTopology: true
  }
).then(() => console.log("Connected to MongoDB"))
.catch(err => console.error("MongoDB connection error:", err));



// Routes
app.use("/player", playerRoutes);

const LevelSchema = new mongoose.Schema({
  levelName: { type: String, required: true, unique: true }, 
  createdAt: { type: Date, default: Date.now }, 
  width: { type: Number, required: true }, 
  height: { type: Number, required: true }, 
  PlacedTiles: [
      {
          X: { type: Number, required: true }, 
          Y: { type: Number, required: true }, 
          TileIndex: { type: Number, required: true } 
      }
  ]
});

const Level = mongoose.model("Level", LevelSchema);


app.post("/levels", async (req, res) => {
  try {
      const { levelName, width, height, PlacedTiles } = req.body;

      const existingLevel = await Level.findOne({ levelName });
      if (existingLevel) {
          return res.status(400).json({ error: "Level name already exists. Choose a different name." });
      }

      const newLevel = new Level({ levelName, width, height, PlacedTiles });
      await newLevel.save();

      res.status(201).json({ message: "Level saved successfully!", level: newLevel });
  } catch (error) {
      res.status(500).json({ error: "Error saving level", details: error.message });
  }
});

app.get("/levels", async (req, res) => {
  try {
      const levels = await Level.find().sort({ createdAt: -1 }); // Get all levels sorted by latest created
      res.json(levels);
  } catch (error) {
      res.status(500).json({ error: "Error retrieving levels", details: error.message });
  }
});

app.get("/levels/:levelName", async (req, res) => {
  try {
      const level = await Level.findOne({ levelName: req.params.levelName });

      if (!level) {
          return res.status(404).json({ error: "Level not found" });
      }

      res.json(level);
  } catch (error) {
      res.status(500).json({ error: "Error retrieving level", details: error.message });
  }
});

app.delete("/levels/:levelName", async (req, res) => {
  try {
      const deletedLevel = await Level.findOneAndDelete({ levelName: req.params.levelName });

      if (!deletedLevel) {
          return res.status(404).json({ error: "Level not found" });
      }

      res.json({ message: "Level deleted successfully!", deletedLevel });
  } catch (error) {
      res.status(500).json({ error: "Error deleting level", details: error.message });
  }
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`Running on port ${PORT}`));
