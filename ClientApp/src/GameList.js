import React, { Component } from 'react'
import GameListElement from './GameListElement.js'

class GameList extends Component {
    constructor(props) {
        super(props);

        this.state = { games: [] }
    }
	
	componentDidMount() {
		this.populateGameList();
	}
	
	showGame(gameCode) {
		this.props.showGameHandler(gameCode);
	}
	
	render() {
        return (
            <ul>
				{this.state.games.map((game, index) => {
					return <GameListElement key={index}
											code={game.code}
											admin={game.adminName} 
											players={game.playerCount} 
											status={game.status} 
											joinHandler={this.showGame.bind(this)} />
				})}
            </ul>
        )
    }
	
	async populateGameList() {
		const response = await fetch('games/');
		const games = await response.json();
		this.setState((state) => {
            return { games: games };
        });
	}
}

export default GameList;