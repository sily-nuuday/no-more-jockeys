import React, { Component } from 'react'
import PlayerList from './PlayerList.js'

class Game extends Component {
    constructor(props) {
        super(props);

        this.state = { game: null }

        this.updateGame = this.updateGame.bind(this);
    }

    componentDidMount() {
        this.props.connection.on("GameUpdated", this.updateGame);
    }

    componentWillUnmount() {
        this.props.connection.off("GameUpdated");

        this.props.connection.invoke("LeaveGame", this.state.game.code).catch(function (err) {
            return console.error(err.toString());
        });
    }

    updateGame(game) {
        this.setState({
            game: game
        });
    }
	
    showGameList() {
        this.props.showGameListHandler();
	}
	
    render() {
        var element;
        if (this.state.game == null) {
            element = <div>Loading...</div>;
        } else {
            element = (
                <div>
                    <PlayerList game={this.state.game} />
                    <button onClick={this.showGameList.bind(this)}>Leave</button>
                </div>
            );
        }
        return element;
    }
}

export default Game;