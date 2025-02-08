import PropTypes from "prop-types";
import "../styles/table.css";

function TabelaLancamentos({ lancamentos, handleSort, sortConfig, handleEdit, handleDelete }) {
  return (
    <div className="table-container">
      <table className="table">
        <thead>
          <tr>
            <th
              className={sortConfig.key === "id" ? `sorted-${sortConfig.direction}` : ""}
              onClick={() => handleSort("id")}
            >
              ID
            </th>
            <th
              className={sortConfig.key === "data" ? `sorted-${sortConfig.direction}` : ""}
              onClick={() => handleSort("data")}
            >
              Data
            </th>
            <th
              className={sortConfig.key === "valor" ? `sorted-${sortConfig.direction}` : ""}
              onClick={() => handleSort("valor")}
            >
              Valor
            </th>
            <th
              className={sortConfig.key === "descricao" ? `sorted-${sortConfig.direction}` : ""}
              onClick={() => handleSort("descricao")}
            >
              Descrição
            </th>
            <th
              className={sortConfig.key === "tipo" ? `sorted-${sortConfig.direction}` : ""}
              onClick={() => handleSort("tipo")}
            >
              Tipo
            </th>
            <th
              className={sortConfig.key === "categoria" ? `sorted-${sortConfig.direction}` : ""}
              onClick={() => handleSort("categoria")}
            >
              Categoria
            </th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {lancamentos.map((lanc) => (
            <tr key={lanc.id}>
              <td>{lanc.id}</td>
              <td>{lanc.data}</td>
              <td>{lanc.valor}</td>
              <td>{lanc.descricao}</td>
              <td>{lanc.tipo}</td>
              <td>{lanc.categoria}</td>
              <td>
                <button className="btn btn-warning" onClick={() => handleEdit(lanc)}>
                  Editar
                </button>
                <button className="btn btn-danger" onClick={() => handleDelete(lanc.id)}>
                  Excluir
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
TabelaLancamentos.propTypes = {
  lancamentos: PropTypes.arrayOf(
    PropTypes.shape({
      id: PropTypes.number.isRequired,
      data: PropTypes.string.isRequired,
      valor: PropTypes.number.isRequired,
      descricao: PropTypes.string.isRequired,
      tipo: PropTypes.string.isRequired,
      categoria: PropTypes.string.isRequired,
    })
  ).isRequired,
  handleSort: PropTypes.func.isRequired,
  sortConfig: PropTypes.shape({
    key: PropTypes.string,
    direction: PropTypes.string,
  }).isRequired,
  handleEdit: PropTypes.func.isRequired,
  handleDelete: PropTypes.func.isRequired,
};

export default TabelaLancamentos;

