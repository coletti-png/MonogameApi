const express = require("express");
const { nanoid } = require("nanoid");
const Player = require("../models/Player");

const router = express.Router();

//#region get all Players
router.get("/", async (req, res) => 
    {
    try 
    {
        const players = await Player.find().sort(
            { 
                screenName: 1 
            });
        res.json(players);
    } 

    catch (error) 
    {
        res.status(500).json(
            { 
                error: "Failed to retrieve players" 
            });
    }
});
//#endregion

//#region get Player by ID
router.get("/:playerid", async (req, res) => 
    {
    try 
    {
        const player = await Player.findOne({ playerid: req.params.playerid });
        if (!player) 
        {
            return res.status(404).json({ error: "Player not found" });
        }
        res.json(player);
    } 

    catch (error) 
    {
        res.status(500).json(
            { 
                error: "Failed to retrieve player" 
            });
    }
});
//#endregion

//#region add Player
router.post("/", async (req, res) => 
    {
    try 
    {
        const newPlayer = new Player(
            {
                playerid: nanoid(8),
                ScreenName: req.body.ScreenName,
                FirstName: req.body.FirstName,
                LastName: req.body.LastName,
                DateStartedPlaying: req.body.DateStartedPlaying,
                Score: req.body.Score
            });

        await newPlayer.save();
        res.json(
            {
                 message: "Player added successfully", playerid: newPlayer.playerid 
            });
    } 

    catch (error) 
    {
        res.status(500).json(
            { 
                error: "Failed to add player->Route" 
            });
    }
});
//#endregion

//#region update Player
router.put("/:playerid", async (req, res) => 
    {
    try {
        const player = await Player.findOne(
            { 
                playerid: req.params.playerid 
            });
        if (!player)
            {
                 return res.status(404).json(
                { 
                    message: "Player not found" 

                });
            }

        Object.assign(player, req.body);
        await player.save();

        res.json(
            {
                 message: "Player updated", player
            });
    } 

    catch (error) 
    {
        res.status(500).json({ error: "Failed to update player" });
    }
});
//#endregion

//#region delete Player
router.delete("/:playerid", async (req, res) => 
    {
    try 
    {
        await Player.deleteOne({ playerid: req.params.playerid });
        res.json(
        { 
            message: "Player deleted successfully" 

        });
    } 
    
    catch (error) 
    {
        res.status(500).json(
            {
                 error: "Failed to delete player" 
            });
    }
});
//#endregion

module.exports = router;