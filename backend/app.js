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

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`Running on port ${PORT}`));
