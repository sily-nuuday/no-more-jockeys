import React, { Component } from 'react'
import GameListElement from './GameListElement.js'

class GameList extends Component {
    constructor(props) {
        super(props);

		this.state = { games: [] }

        this.updateGameList = this.updateGameList.bind(this);
        this.joinGame = this.joinGame.bind(this);
    }

    componentDidMount() {
        this.props.connection.on("GameListUpdated", this.updateGameList);

        this.props.connection.invoke("RetrieveGameList").catch(function (err) {
            return console.error(err.toString());
        });
    }

    componentWillUnmount() {
        this.props.connection.invoke("UnsubscribeGameList").catch(function (err) {
            return console.error(err.toString());
        });

        this.props.connection.off("GameListUpdated");
    }

	updateGameList(games) {
        this.setState({
            games: games
        });
    }
	
    joinGame(gameCode) {
        this.props.joinHandler(gameCode);

        this.props.connection.invoke("JoinGame", this.props.name, gameCode).catch(function (err) {
            return console.error(err.toString());
        });
    }
	
	render() {
        return (
            <ul className="game-list left">
				{this.state.games.map((game, index) => {
                    return <GameListElement key={index}
											code={game.code}
											admin={game.adminName} 
											players={game.playerCount} 
											status={game.status} 
											joinHandler={this.joinGame} />;
                })}
            </ul>
        );
    }
}

export default GameList;