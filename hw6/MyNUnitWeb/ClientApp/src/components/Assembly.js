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
import { useNavigate } from "react-router-dom";

function withParams(Component) {
  return (props) => <Component {...props} params={useParams()} navigation ={useNavigate()}/>;
}

class Assembly extends Component {
  static displayName = Assembly.name;

  constructor(props) {
    super(props);
    this.state = { files: [], loading: true, selectedFiles: [] };
  }

  componentDidMount() {
    this.state.id = this.props.params.id;
    this.populateData();
  }

  static renderTable(files) {
    return (
      <table className="table table-striped" aria-labelledby="tableLabel">
        <thead>
          <tr>
            <th>Имя файла</th>
            <th>SHA-256</th>
          </tr>
        </thead>
        <tbody>
          {files.map((file) => (
            <tr key={file.id}>
              <td>{file.name}</td>
              <td>{file.contentSha256}</td>
            </tr>
          ))}
        </tbody>
      </table>
    );
  }

  render() {
    const handleFileChange = (event) => {
      this.state.selectedFiles = [...event.target.files];
    };

    const handleUpload = () => {
      const formData = new FormData();
      this.state.selectedFiles.forEach((file) => {
        formData.append("files", file);
      });

      fetch(`assembly/${this.state.id}/files`, {
        method: "POST",
        body: formData,
      }).then(() => this.populateData());
    };

    const runTests = () => {
      fetch(`assembly/${this.state.id}/runTest`, {
        method: "POST"
      })
      .then((response) => response.json())
      .then((data) => this.props.navigation(`/test/${data.id}`));
    }

    let contents = this.state.loading ? (
      <p>
        <em>Loading...</em>
      </p>
    ) : (
      Assembly.renderTable(this.state.files)
    );

    return (
      <div>
        <h1 id="tableLabel">Сборка {this.state.id}</h1>
          <button onClick={runTests}>Запустить тесты</button>
        {contents}
        <div>
          <input type="file" multiple onChange={handleFileChange} />
          <button onClick={handleUpload}>Загрузить файл</button>
        </div>
      </div>
    );
  }

  async populateData() {
    const response = await fetch(`assembly/${this.state.id}`);
    const data = await response.json();
    this.setState({ files: data.files, loading: false });
  }
}

export default withParams(Assembly);
