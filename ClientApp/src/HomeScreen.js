import React, { Component } from 'react'
import GameList from './GameList.js'
import CreateGame from './CreateGame.js'

class HomeScreen extends Component {
    constructor(props) {
		super(props);

        this.state = { name: "" };

        this.updateName = this.updateName.bind(this);
        this.showGame = this.showGame.bind(this);
	}

	updateName(event) {
        this.setState({
            name: event.target.value
        });
    }

	showGame(gameCode) {
		this.props.showGameHandler(gameCode);
	}
	
	render() {
        return (
			<div className="fit-content">
				<div className="align-center">
					<span>Name: <input type="text" value={this.state.name} onChange={this.updateName} /></span>
				</div>
				<GameList name={this.state.name} joinHandler={this.showGame} connection={this.props.connection} />
				<CreateGame name={this.state.name} showGameHandler={this.showGame} connection={this.props.connection} />
			</div>
        );
    }
}

export default HomeScreen;