import React, { Component } from "react";
import {
  Collapse,
  Navbar,
  NavbarBrand,
  NavbarToggler,
  NavItem,
  NavLink,
} from "reactstrap";
import { Link, useParams } from "react-router-dom";

function withParams(Component) {
  return (props) => <Component {...props} params={useParams()} />;
}

class Test extends Component {
  static displayName = Test.name;

  constructor(props) {
    super(props);
    this.state = { tests: [], loading: true };
  }

  componentDidMount() {
    this.id = this.props.params.id;
    this.populateData();
  }

  static renderTable(tests) {
    return (
      <table className="table table-striped" aria-labelledby="tableLabel">
        <thead>
          <tr>
            <th>Тест</th>
            <th>Результат</th>
            <th>Время работы</th>
            <th>Текст ошибки/причина игнорирования</th>
          </tr>
        </thead>
        <tbody>
          {tests.map((test) => (
            <tr key={test.id}>
              <td>
                {test.className}.{test.methodName}
              </td>
              {test.exception !== null ? (
                <td>Ошибка</td>
              ) : test.reason !== null ? (
                <td>Проигнорирован</td>
              ) : (
                <td>Ок</td>
              )}
              <td>{test.elapsed ?? "-"}</td>
              <td>{test.exception ?? test.reason ?? "-"}</td>
            </tr>
          ))}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading ? (
      <p>
        <em>Loading...</em>
      </p>
    ) : (
      Test.renderTable(this.state.tests.testResults)
    );

    return (
      <div>
        <h1 id="tableLabel">Тест {this.props.params.id}</h1>
        {contents}
      </div>
    );
  }

  async populateData() {
    const response = await fetch(`tests/${this.id}`);
    const data = await response.json();
    this.setState({ tests: data, loading: false });
  }
}

export default withParams(Test);
