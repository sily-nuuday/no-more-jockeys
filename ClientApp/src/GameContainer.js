import React, { Component } from 'react'
import GameList from './GameList.js'
import Game from './Game.js'

class GameContainer extends Component {
	constructor(props) {
        super(props);
        this.state = { gameCode: null };
    }
	
	showGameList() {
		this.showGame(null);
	}
	
	showGame(gameCode) {
		this.setState((state) => {
            return { gameCode: gameCode };
        });
	}
	
	render() {
        const isGameLoaded = this.state.gameCode !== null;
        return (
            <div>
                {isGameLoaded
                ? <Game gameCode={this.state.gameCode} showGameListHandler={this.showGameList.bind(this)}/>
                : <GameList showGameHandler={this.showGame.bind(this)} />}
            </div>
        )
    }
}

export default GameContainer;